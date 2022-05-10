using Core.Common;
using Core.Enums;
using Core.Models;
using Core.ServiceBus;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Application.PurchaseOrders.Queries;
using Tradefact.Application.Schedules.Model;
using Tradefact.Data;

namespace Tradefact.Application.FreightMovements
{
    public class CreateFreightMovementCommand: FreightMovementCommand, IRequest<Guid>
    {
        [JsonIgnore]
        public Guid OrganisationId { get; set; }
    }

    public class CreateFreightMovementCommandHandler : IRequestHandler<CreateFreightMovementCommand, Guid>
    {
        private readonly IMediator _mediator;
        private readonly TradefactDbContext _context;
        private readonly ILogger<CreateFreightMovementCommandHandler> _logger;
        private readonly IServiceBusClient _serviceBusClient;

        // Using DI to inject infrastructure persistence Repositories
        public CreateFreightMovementCommandHandler(IMediator mediator, TradefactDbContext context, ILogger<CreateFreightMovementCommandHandler> logger, IServiceBusClient serviceBusClient)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceBusClient = serviceBusClient ?? throw new ArgumentNullException(nameof(serviceBusClient));
        }

        public async Task<Guid> Handle(CreateFreightMovementCommand command, CancellationToken cancellationToken)
        {

            FreightMovement fm = new FreightMovement();
            fm = command.Adapt<FreightMovement>();
            fm.Reference = this.GenerateReference();

            fm.CompanyId = command.OrganisationId;
            fm.Id = Guid.NewGuid();

            // Road - Potentially now recieving a empty string - make null to ensure FK
            fm.PortOfLoadingId = (!String.IsNullOrEmpty(fm.PortOfLoadingId)) ? fm.PortOfLoadingId : null;
            fm.PortOfDischargeId = (!String.IsNullOrEmpty(fm.PortOfDischargeId)) ? fm.PortOfDischargeId : null;

            fm.Items = new List<FreightMovementItem>();

            List<PurchaseOrderScheduleLineResource> lines = new List<PurchaseOrderScheduleLineResource>();
            List<KeyValuePair<Guid, Address>> placesofLoading = new List<KeyValuePair<Guid, Address>>();

            foreach (Guid scheduleId in command.AttachedSchedules )
            {
                PurchaseOrderScheduleLineResource schedule = await _mediator.Send(new GetPurchaseOrderScheduleByIdQuery
                {
                    ScheduleLineId = scheduleId,
                    IncludeItems = true,
                    Paging = new PagedResultParameters { NoPage = true, PageNumber = 1, PageSize = int.MaxValue }
                });
                command.FCL.Add(this.BuildFCLFromSchedule(schedule));

                if (schedule.GoodsReady > fm.GoodsReady) fm.GoodsReady = schedule.GoodsReady.GetValueOrDefault();

                // PMC - Treating all schedules as FCL for storage purposes
                //if (fm.LoadType == LoadTypeEnum.LCL)
                //{
                //    command.LCL.AddRange(this.BuildLCLFromSchedule(schedule));
                //}
                //else
                //{
                //    command.FCL.Add(this.BuildFCLFromSchedule(schedule));
                //}

                if (!placesofLoading.Any(q => q.Key == schedule.PlaceOfLoadingId)) placesofLoading.Add(new KeyValuePair<Guid, Address>(schedule.PlaceOfLoadingId,   schedule.PlaceOfLoading));
                lines.Add(schedule);
            }
            
            if (placesofLoading.Count > 0)
            {
                fm.PlaceOfLoadingId = placesofLoading[0].Key;
                if (placesofLoading.Count > 1)
                {
                    Address a = placesofLoading[0].Value;
                    fm.PlaceOfLoadingMultiple = $" ({a.Country.Code2}) {a.Country.Name} * {placesofLoading.Count}";
                }
            }

            foreach (var fcl in command.FCL.Where(q=>q.CargoItems.Count > 0))
            {
                FreightMovementItem container = new FreightMovementItem
                {
                    Id = Guid.NewGuid(),
                    ContainerTypeCode = fcl.ContainerType,
                    HazardCode = fcl.HazardCode,
                    CargoItems = new List<CargoItem>(),
                    PurchaseOrderId = fcl.PurchaseOrderId,
                    PurchaseOrderItemScheduleLineId = fcl.PurchaseOrderItemScheduleLineId
                };
                foreach (var item in fcl.CargoItems)
                {
                    CargoItem c = new CargoItem
                    {
                        ItemId = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        IsProductVariant = item.IsProductVariant,
                        HsCode = item.HsCode,
                        SKU = item.SKU,
                        CartonQty = item.CartonQty,
                        Qty = item.Qty > 0 ? item.Qty : item.CartonQty,
                        Width = item.Width,
                        Length = item.Length,
                        Height = item.Height,
                        UOL = item.UOL,
                        UOW = item.UOW,
                        Weight = item.Weight,
                        PurchaseOrderId = item.PurchaseOrderId,
                        PurchaseOrderItemId = item.PurchaseOrderItemId,
                        PurchaseOrderItemScheduleLineId = item.PurchaseOrderItemScheduleLineId,
                        ItemDescription = item.ItemDescription
                    };

                    if (c.IsProductVariant)
                    {
                        c.ProductVariantId = c.ProductId;
                        c.ProductId = await this.GetProductVariantMasterId(item.ProductId);
                    }

                    container.CargoItems.Add(c);
                    fm.ConsignmentQuantity += c.Qty;
                }

                fm.Items.Add(container);
            }
            foreach (var item in command.LCL)
            {
                FreightMovementItem container = new FreightMovementItem
                {
                    Id = Guid.NewGuid(),
                    HazardCode = item.HazardCode,
                    CartonQty = item.CartonQty,
                    CargoItems = new List<CargoItem>(),
                    PurchaseOrderId = item.PurchaseOrderId,
                    PurchaseOrderItemScheduleLineId = item.PurchaseOrderItemScheduleLineId

                };

                CargoItem c = new CargoItem
                {
                    ItemId = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    IsProductVariant = item.IsProductVariant,
                    HsCode = item.HsCode,
                    SKU = item.SKU,
                    CartonQty = item.CartonQty,
                    Qty = item.Qty > 0 ? item.Qty : item.CartonQty,
                    Width = item.Width,
                    Length = item.Length,
                    Height = item.Height,
                    UOL = item.UOL,
                    UOW = item.UOW,
                    Weight = item.Weight,
                    PurchaseOrderId = item.PurchaseOrderId,
                    PurchaseOrderItemId = item.PurchaseOrderItemId,
                    PurchaseOrderItemScheduleLineId = item.PurchaseOrderItemScheduleLineId,
                    ItemDescription = item.ItemDescription
                };

                if (c.IsProductVariant)
                {
                    c.ProductVariantId = c.ProductId;
                    c.ProductId = await this.GetProductVariantMasterId(item.ProductId);
                }

                container.CargoItems.Add(c);
                fm.Items.Add(container);

                fm.ConsignmentQuantity += c.Qty;
            }

            if (command.ForwarderList == null || command.ForwarderList.Count == 0)
            {
                Partnership partnership = await _context.Partnerships.Include(i => i.Provider).FirstOrDefaultAsync(q => q.ClientId == command.OrganisationId);
                command.ForwarderList = new List<Recipient>();
                command.ForwarderList.Add(new Recipient()
                {
                    PartnershipId = partnership.Id,
                    Type = Core.Enums.RecipientType.TradefactForwarder
                });
            }

            var shipper = _context.Organisations.FirstOrDefault(x => x.Id == command.OrganisationId);

            foreach (var forwarder in command.ForwarderList.Where(x => x.Type == Core.Enums.RecipientType.TradefactForwarder))
            {
                Partnership partnership = await _context.Partnerships.Include(i => i.Provider).FirstOrDefaultAsync(q => q.Id == forwarder.PartnershipId);

                fm.QuotationRequests.Add(new QuotationRequest
                {
                    PartnershipId = forwarder.PartnershipId,
                    Submitted = DateTime.Now,
                    State = Core.Enums.QuotationStateEnum.PENDING
                });
                SendMessageToServiceBus(fm, partnership.Provider, shipper);
            }

            foreach (var forwarder in command.ForwarderList.Where(x => x.Type == Core.Enums.RecipientType.EmailForwarder))
            {
                //PRepare Email Public link to send to forwader.Email
            }

            _context.FreightMovements.Add(fm);
            _ = await _context.SaveChangesAsync();

            return fm.Id;
        }

        private FCLItem BuildFCLFromSchedule(PurchaseOrderScheduleLineResource s)
        {
            FCLItem fcl = new FCLItem() { CargoItems = new List<CargoItem>(), ContainerType = "00G0", PurchaseOrderId = s.PurchaseOrderId, PurchaseOrderItemScheduleLineId = s.ScheduleLineId };
            foreach (var item in s.Schedule)
            {
                fcl.CargoItems.Add(
                    new CargoItem
                    {
                        ItemId = Guid.NewGuid(),
                        ProductId = Guid.Parse(item.Product.Id),
                        IsProductVariant = false,
                        HsCode = item.Product.HsCode,
                        SKU = item.Product.SKU,
                        ItemDescription = item.Product.Name,
                        ProductDescriptionOverride = false,
                        CartonQty = 1,
                        Qty = item.ScheduleLineCommittedQuantity,
                        Width = item.Product.Dimensions.Width,
                        Length = item.Product.Dimensions.Length,
                        Height = item.Product.Dimensions.Height,
                        Weight = item.Product.Dimensions.Weight,

                        PurchaseOrderId = s.PurchaseOrderId,
                        PurchaseOrderItemId = item.PurchaseOrderItemId,
                        PurchaseOrderItemScheduleLineId = s.ScheduleLineId,
                        PlaceOfLoadingId = s.PlaceOfLoadingId,
                    }    
                );
            }
            return fcl;
        }

        private async void SendMessageToServiceBus(FreightMovement fm, Core.Models.Organisation org, Core.Models.Organisation shipper)
        {
            string QueueName = "quotequeue";

            var user = _context.Users.SingleOrDefault(x => x.OrganisationId == org.Id && x.Email == org.ContactEmail);

            ServiceBusMessage<QuoteCreatedMail> msg = new ServiceBusMessage<QuoteCreatedMail>(new QuoteCreatedMail
            {
                Email = org.ContactEmail,
                OrganisationName = shipper.Name,
                FirstName = user?.GivenName
            });

            await _serviceBusClient.Publish<QuoteCreatedMail>(msg, QueueName);
        }
        private IEnumerable<LCLItem> BuildLCLFromSchedule(PurchaseOrderScheduleLineResource s)
        {
            foreach (var item in s.Schedule)
            {
                yield return
                    new LCLItem
                    {
                        ItemId = Guid.NewGuid(),
                        ProductId = Guid.Parse(item.Product.Id),
                        IsProductVariant = false,
                        HsCode = item.Product.HsCode,
                        SKU = item.Product.SKU,
                        ItemDescription = item.Product.Name,
                        ProductDescriptionOverride = false,
                        CartonQty = 1,
                        Qty = item.ScheduleLineCommittedQuantity,
                        Width = item.Product.Dimensions.Width,
                        Length = item.Product.Dimensions.Length,
                        Height = item.Product.Dimensions.Height,
                        Weight = item.Product.Dimensions.Weight,

                        PurchaseOrderId = item.PurchaseOrderId,
                        PurchaseOrderItemId = item.PurchaseOrderItemId,
                        PurchaseOrderItemScheduleLineId = item.PurchaseOrderItemScheduleLineId,
                        PlaceOfLoadingId = item.PlaceOfLoadingId
                    };
            };
            yield break;
        }

        private string GenerateReference()
        {
            DateTime _now = DateTime.Now;

            StringBuilder builder = new StringBuilder();
            builder.Append(_now.ToString("yy"));
            builder.Append("-");
            builder.Append(_now.ToString("MM"));
            builder.Append("-");

            Random random = new Random();
            char ch;
            for (int i = 0; i < 5; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }

        private async Task<Guid> GetProductVariantMasterId(Guid variantId)
        {
            ProductVariant variant = await _context.ProductVariants.SingleOrDefaultAsync(s => s.Id == variantId);
            return variant.ProductId;
        }
    }

    public class FreightMovementCommand
    {
        public Guid Id { get; set; }

        // BASIC
        public string Name { get; set; }
        private string Reference { get; set; }

        public TransactionTypeEnum TransactionType { get; set; }
        public ShipmentTypeEnum ShipmentMethod { get; set; }
        public IncoTypeEnum IncoTerms { get; set; }

        // ORIGIN
        public string PlaceOfLoading { get; set; }
        public string PortOfLoading { get; set; }
        public DateTime GoodsReady { get; set; }

        // DESTINATION
        public string PortOfDischarge { get; set; }
        public string PlaceOfDispatch { get; set; }
        public DateTime? DeliveryDate { get; set; }

        // CARGO
        public LoadTypeEnum LoadType { get; set; }

        public List<FCLItem> FCL { get; set; }
        public List<LCLItem> LCL { get; set; }
        public List<string> HSCodes { get; set; }

        public List<Guid> AttachedSchedules { get; set; }

        public bool? InsuranceRequired { get; set; } = null;
        public string InsuranceCurrency { get; set; }
        public decimal? InsuranceValue { get; set; } = 0;

        public bool? CustomsBrokerageRequired { get; set; } = null;
        public int? NumberOfItems { get; set; } = 1;

        public Guid? SupplierId { get; set; }
        public Guid? BuyerId { get; set; }

        public List<string> Tags { get; set; }

        public string Notes { get; set; }

        public List<Recipient> ForwarderList { get; set; } = null;

        public string TagList => (Tags != null && Tags.Any()) ? String.Join(',', Tags.ToArray()) : null;
        public string HsCodeList => (HSCodes != null && HSCodes.Count > 0) ? String.Join(',', HSCodes.ToArray()) : null;

    }

    public class CreateFreightMovementCommandValidator : AbstractValidator<CreateFreightMovementCommand>
    {
        public CreateFreightMovementCommandValidator()
        {
            // RuleFor(x => x.Reference).NotEmpty().MinimumLength(3);

            RuleFor(x => x.LCL).Custom((list, context) => {
                if (list.Count > 1000)
                {
                    context.AddFailure("The list must contain 10 items or fewer");
                }
            });
            RuleForEach(x => x.LCL).SetValidator(new LCLValidator());
        }

        private class LCLValidator : AbstractValidator<CargoItem>
        {
            public LCLValidator()
            {
                // All your other validation rules for Guitar. eg.
                RuleFor(x => x.ProductId).NotNull();

                RuleFor(x => x.HsCode).NotNull();
                RuleFor(x => x.SKU).NotNull();

                RuleFor(x => x.Qty).GreaterThanOrEqualTo(0);
                RuleFor(x => x.Length).GreaterThanOrEqualTo(0);
                RuleFor(x => x.Width).GreaterThanOrEqualTo(0);
                RuleFor(x => x.Height).GreaterThanOrEqualTo(0);
            }
        }

        private class FCLValidator : AbstractValidator<FCLItem>
        {
            public FCLValidator()
            {
                // All your other validation rules for Guitar. eg.
                RuleForEach(x => x.CargoItems).SetValidator(new LCLValidator());
            }
        }


    }
}
