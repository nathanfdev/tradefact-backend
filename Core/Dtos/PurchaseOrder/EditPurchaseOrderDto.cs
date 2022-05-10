using Core.Enums;
using Core.Models;
using FluentValidation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Dtos.PurchaseOrder
{

    public class PurchaseOrderEditDto
    {
        /// <summary>Supplier Id</summary>
        //[JsonProperty("PurchaseOrderId")]
        //public Guid PurchaseOrderId { get; set; }

        [JsonProperty("SupplierId")]
        public Guid SupplierId { get; set; }

        [JsonProperty("PONumber")]
        public string PurchaseOrderNumber { get; set; }

        [JsonProperty("Reference")]
        public string Reference { get; set; }

        [JsonProperty("PurchaseOrderDate")]
        public DateTime? PurchaseOrderDate { get; set; }

        [JsonProperty("GoodsReadyDate")]
        public DateTime? GoodsReadyDate { get; set; }

        [JsonProperty("TargetDeliveryDate")]
        public DateTime? TargetDeliveryDate { get; set; }

        [JsonProperty("IncotermsVersion")]
        public string IncotermsVersion { get; set; }

        public ShipmentTypeEnum ShipmentType { get; set; }
        public IncoTypeEnum IncoTerms { get; set; }
        // Cargo Type
        public LoadTypeEnum LoadType { get; set; }
        public TransactionTypeEnum TransactionType { get; set; }

        // Only for EXW
        public Guid? PlaceOfLoadingId { get; set; }
        public string PortOfLoadingId { get; set; }
        public string PortOfDischargeId { get; set; }
        public Guid? PlaceOfDispatchId { get; set; }

        [JsonProperty("Language")]
        public string Language { get; set; }

        [JsonProperty("PurchasingGroup")]
        public string PurchasingGroup { get; set; }

        [JsonProperty("CorrespncExternalReference")]
        public string CorrespncExternalReference { get; set; }

        [JsonProperty("CorrespncInternalReference")]
        public string CorrespncInternalReference { get; set; }

        public decimal TaxRate { get; set; }
        public string TaxDescription { get; set; }

        public int ValidForDays { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public string PaymentTerms { get; set; }

        public bool? CustomsBrokerageRequired { get; set; } = null;
        public int? NumberOfItems { get; set; } = 1;

        // Insurance
        public bool? InsuranceRequired { get; set; } = null;
        public string InsuranceCurrency { get; set; }
        public decimal? InsuranceValue { get; set; } = 0;

        [JsonIgnore]
        public string Tags => (TagList != null && this.TagList.Any()) ? String.Join(',', this.TagList.ToArray()) : null;

        [JsonProperty("Tags")]
        public List<string> TagList { get; set; }

        [JsonIgnore]
        public string Containers => (ContainerList != null && this.ContainerList.Any()) ? String.Join(',', this.ContainerList.ToArray()) : null;

        [JsonProperty("Containers")]
        public List<string> ContainerList { get; set; }

        [JsonProperty("HSCodes")]
        public List<string> HSCodesList { get; set; }

        [JsonIgnore]
        public string HSCodes => (HSCodesList != null && this.HSCodesList.Any()) ? String.Join(',', this.HSCodesList.ToArray()) : null;

        public string CurrencyId { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal InverseExchangeRate { get; set; }

        public string Notes { get; set; }
        public string Signature { get; set; }
        public string AdditionalInformation { get; set; }

        public Total Total { get; set; }
        public Total BaseCurrency { get; set; }

        public string LogisticsNotes { get; set; }
        public string AdditionalSupplierInformation { get; set; }
    }

    public class PurchaseOrderItemEditDto
    {
        [JsonProperty("ProductId")]
        public Guid? ProductId { get; set; }

        [JsonProperty("OrderQuantity")]
        public decimal? OrderQuantity { get; set; }

        [JsonProperty("PurchaseOrderQuantityUnit")]
        public string OrderQuantityUnit { get; set; }

        [JsonProperty("OrderPriceUnit")]
        public decimal? OrderPriceUnit { get; set; }

        [JsonProperty("supplierReference")]
        public string SupplierReference { get; set; }
    }

    public class PurchaseOrderItemCreateDto
    {
        [JsonProperty("ProductId")]
        public Guid ProductId { get; set; }
    }

    public class PurchaseOrderEditDtoValidator : AbstractValidator<PurchaseOrderEditDto>
    {
        public PurchaseOrderEditDtoValidator()
        {
            RuleFor(x => x.SupplierId).NotNull();

            RuleFor(x => x.IncoTerms).IsInEnum();
            RuleFor(x => x.LoadType).IsInEnum();
            RuleFor(x => x.TransactionType).IsInEnum();
            RuleFor(x => x.ShipmentType).IsInEnum();

            RuleFor(x => x.GoodsReadyDate)
                .NotEmpty().WithMessage("Required field")
                .GreaterThanOrEqualTo(p => DateTime.Now.ToUniversalTime().Date).WithMessage("Goods ready date must not be in the past");

            RuleFor(x => x.PlaceOfLoadingId).NotNull().NotEmpty();
            RuleFor(x => x.PlaceOfDispatchId).NotNull().NotEmpty();

            RuleFor(x => x.PortOfLoadingId).NotNull().NotEmpty();
            RuleFor(x => x.PlaceOfDispatchId).NotNull().NotEmpty();
        }
    }

}
