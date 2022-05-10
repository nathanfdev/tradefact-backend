using Core.Common;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Flare.Api.Model;
using Flare.Data;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.Application.Models;
using X.PagedList;

namespace Flare.Api.Controllers
{
    public abstract class OrganisationController : BaseApiController
    {
        protected readonly IMediator _mediator;
        protected readonly ILocationService _locationService;

        protected virtual OrganisationTypeEnum OrganisationType => OrganisationTypeEnum.SHIPPER;

        public OrganisationController(FlareDbContext context, IMediator mediator, ILocationService locationService) : base(context)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(OrganisationResource), Description = "OK Result")]
        public async Task<IActionResult> Get()
        {
            Organisation org = await _context.Organisations.Include(i=>i.Addresses).ThenInclude(i=>i.Country).SingleAsync(q => q.Id == this.OrganisationId);
            if (org == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(org.Adapt<OrganisationResource>());
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(OrganisationResource), Description = "Created result")]
        public async Task<IActionResult> Post([FromBody] CreateOrganisationRequest request)
        {
            var org = new Organisation { Id = Guid.NewGuid(), Name = request.Name, PaymentTerms = 30, OrganisationTypeId = this.OrganisationType };
            _context.Organisations.Add(org);
            _ = await _context.SaveChangesAsync();

            return new OkObjectResult(org.Adapt<OrganisationResource>());
        }

        //[HttpPost]
        //[SwaggerResponse("200", typeof(Organisation), Description = "Updated result")]
        //[Route(nameof(Update))]
        //public async Task<IActionResult> Update([FromBody]UpdateCompanyRequest request)
        //{
        //    Organisation org = await _context.Organisations.SingleAsync(q => q.Id == this.OrganisationId && q.OrganisationTypeId == this.OrganisationType);
        //    org = request.Adapt<Organisation>();
        //    _ = await _context.SaveChangesAsync();
        //    return new OkObjectResult(org);
        //}

        [HttpPut]
        [SwaggerResponse("200", typeof(OrganisationResource), Description = "Updated result")]
        public async Task<IActionResult> Update([FromBody]UpdateCompanyRequest request)
        {
            Organisation org = await _context.Organisations.SingleAsync(q => q.Id == this.OrganisationId && q.OrganisationTypeId == this.OrganisationType);

            org = request.Adapt(org);
            _ = await _context.SaveChangesAsync();
            return new OkObjectResult(org.Adapt<OrganisationResource>());
        }

        [HttpPatch]
        [SwaggerResponse("200", typeof(DirectoryResource), Description = "Update result")]
        public virtual async Task<IActionResult> Patch([FromBody] JsonPatchDocument<UpdateCompanyRequest> patchDoc)
        {
            if (patchDoc != null)
            {
                Organisation org = await _context.Organisations.SingleAsync(q => q.Id == this.OrganisationId && q.OrganisationTypeId == this.OrganisationType);
                UpdateCompanyRequest request = new UpdateCompanyRequest();
                request = org.Adapt(request);
                patchDoc.ApplyTo(request, ModelState);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                org = request.Adapt(org);
                _ = await _context.SaveChangesAsync();

                return new ObjectResult(org.Adapt<DirectoryResource>());
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<AddressResource>), Description = "Updated result")]
        [Route("address")]
        public async Task<IActionResult> GetDirectoryAddresses([FromQuery] PagedResultParameters @params, string search = null)
        {
            Organisation org = await _context.Organisations.Include(i => i.Addresses).ThenInclude(address => address.Country).SingleOrDefaultAsync(q => q.Id == this.OrganisationId && q.OrganisationTypeId == this.OrganisationType);
            if (org == null)
            {
                return new NotFoundResult();
            }
            //IQueryable<AddressResource> query = org.Addresses.Where(q => q.IsActive).Adapt<List<AddressResource>>().AsQueryable();
            IQueryable<Address> query = org.Addresses.OrderByDescending(x=>x.LastModifiedOnInternal).Where(q => q.B2BConnectionId == null && q.IsActive).AsQueryable();
            if (search != null && search.Length > 0)
            {
                query = query.Where(q => EF.Functions.Like(q.Name, $"%{search}%") || EF.Functions.Like(q.AddressLine1, $"%{search}%") || EF.Functions.Like(q.City, $"%{search}%"));
                IQueryable<SortedAddressResource> addresses = query.Adapt<List<SortedAddressResource>>().AsQueryable();

                return new OkObjectResult(new ListResource<AddressResource>(addresses.OrderBy(x => x.Position(search)).ThenBy(x => x.Name).ToPagedList(@params.PageNumber, @params.PageSize)));
            }

            return new OkObjectResult(new ListResource<AddressResource>(query.Adapt<List<AddressResource>>().ToPagedList(@params.PageNumber, @params.PageSize)));
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(AddressResource), Description = "Updated result")]
        [Route("address/{addressid:guid}")]
        public async Task<IActionResult> GetDirectoryAddressById(string addressid)
        {
            Organisation org = await _context.Organisations.Include(i => i.Addresses).ThenInclude(address => address.Country).SingleOrDefaultAsync(q => q.Id == this.OrganisationId && q.OrganisationTypeId == this.OrganisationType && q.IsActive);
            if (org == null || org.Addresses == null || !org.Addresses.Any(q => q.Id == new Guid(addressid)))
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(org.Addresses.Single(q => q.Id == new Guid(addressid)).Adapt<AddressResource>());
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(AddressResource), Description = "Updated result")]
        [Route("address/invoice")]
        public async Task<IActionResult> GetInvoiceAddress()
        {
            Organisation org = await _context.Organisations.Include(i => i.Addresses).ThenInclude(address => address.Country).SingleOrDefaultAsync(q => q.Id == this.OrganisationId && q.OrganisationTypeId == this.OrganisationType);
            if (org == null || org.Addresses == null || !org.Addresses.Any(q => q.IsInvoiceAddress))
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(org.Addresses.Single(q => q.IsInvoiceAddress).Adapt<AddressResource>());
        }

        [HttpPost, HttpPut]
        [SwaggerResponse("200", typeof(AddressResource), Description = "Updated result")]
        [Route("address/invoice")]
        public async Task<IActionResult> CreateUpdateInvoiceAddress([FromBody]UpdateAddressRequest request, string id, string addressid)
        {
            Organisation org = await _context.Organisations.Include(i => i.Addresses).SingleOrDefaultAsync(q => q.Id == this.OrganisationId && q.OrganisationTypeId == this.OrganisationType);
            if (org == null || org.Addresses == null || !org.Addresses.Any(q => q.IsInvoiceAddress))
            {
                Address new_invoice_address = new Address();
                new_invoice_address = request.Adapt<Address>();
                new_invoice_address.IsInvoiceAddress = true;
                new_invoice_address.Position = await _locationService.GetAddressLocation(new_invoice_address);
                org.Addresses.Add(new_invoice_address);
                _ = await _context.SaveChangesAsync();
                
                return new OkObjectResult(org);
            }

            Address add = org.Addresses.Single(q => q.IsInvoiceAddress);
            add = request.Adapt(add);
            add.Position = await _locationService.GetAddressLocation(add);

            _ = await _context.SaveChangesAsync();

             org = await _context.Organisations.Include(i => i.Addresses).SingleOrDefaultAsync(q => q.Id == this.OrganisationId && q.OrganisationTypeId == this.OrganisationType);
            return new OkObjectResult(org);
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(AddressResource), Description = "Created result")]
        [Route("address")]
        public async Task<IActionResult> CreateAddress([FromBody]CreateAddressRequest request)
        {
            Organisation org = await _context.Organisations.SingleAsync(q => q.Id == this.OrganisationId && q.OrganisationTypeId == this.OrganisationType);
            if (org == null)
            {
                return new NotFoundResult();
            }
            Address add = request.Adapt<Address>();
            add.Id = new Guid();
            add.Position = await _locationService.GetAddressLocation(add);
            org.Addresses.Add(add);

            _ = await _context.SaveChangesAsync();
            return new OkObjectResult(add.Adapt<AddressResource>());
        }

        [HttpPut]
        [SwaggerResponse("200", typeof(AddressResource), Description = "Updated result")]
        [Route("address/{addressid:guid}")]
        public async Task<IActionResult> UpdateAddress([FromBody]UpdateAddressRequest request, string addressid)
        {
            Address add = await _context.Addresses.SingleOrDefaultAsync(q => q.Id == new Guid(addressid) && q.OrganisationId == this.OrganisationId);
            if (add == null)
            {
                return new NotFoundResult();
            }
            add = request.Adapt(add);
            add.Position = await _locationService.GetAddressLocation(add);
            _ = await _context.SaveChangesAsync();

            add = await _context.Addresses.Include(i => i.Country).SingleOrDefaultAsync(q => q.Id == new Guid(addressid));
            return new OkObjectResult(add.Adapt<AddressResource>());
        }

        [HttpPatch]
        [SwaggerResponse("200", typeof(AddressResource), Description = "Updated result")]
        [Route("address/{addressid:guid}")]
        public async Task<IActionResult> UpdateAddress([FromBody] JsonPatchDocument<UpdateAddressRequest> patchDoc, string addressid)
        {

            if (patchDoc != null)
            {
                Organisation org = await _context.Organisations.Include(i => i.Addresses).SingleOrDefaultAsync(q => q.Id == this.OrganisationId && q.OrganisationTypeId == this.OrganisationType);
                if (org == null || org.Addresses == null || !org.Addresses.Any(q => q.Id == new Guid(addressid)))
                {
                    return new NotFoundResult();
                }
                Address add = org.Addresses.Single(q => q.Id == new Guid(addressid));
                UpdateAddressRequest request = new UpdateAddressRequest();
                request = org.Adapt(request);
                patchDoc.ApplyTo(request, ModelState);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                add = request.Adapt(add);
                add.Position = await _locationService.GetAddressLocation(add);
                _ = await _context.SaveChangesAsync();

                return new ObjectResult(add.Adapt<AddressResource>());
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpDelete]
        [Route("address/{addressid:guid}")]
        public async Task<IActionResult> DeleteAddress(string addressid)
        {
            Organisation org = await _context.Organisations.Include(i => i.Addresses).ThenInclude(address => address.Country).SingleOrDefaultAsync(q => q.Id == this.OrganisationId && q.OrganisationTypeId == this.OrganisationType);
            if (org == null || org.Addresses == null || !org.Addresses.Any(q => q.Id == new Guid(addressid)))
            {
                return new NotFoundResult();
            }
            Address add = org.Addresses.Single(q => q.Id == new Guid(addressid));
            add.IsActive = false;
            _ = await _context.SaveChangesAsync();
            return Ok();
        }

        //[HttpGet()]
        //[SwaggerResponse("200", typeof(ListResource<PartnershipResource>), Description = "OK Result")]
        //[Route("Partnerships")]
        //public async Task<IActionResult> Partners([FromQuery] PagedResultParameters @params, string search = null)
        //{
        //    IPagedList<PartnershipResource> partnerships = await _mediator.Send(new GetLogisticsPartnersListQuery
        //    {
        //        OrganisationId = this.OrganisationId,
        //        OrganisationType = this.OrganisationType,
        //        Search = search,
        //        Paging = @params
        //    });
        //    return new OkObjectResult(new ListResource<PartnershipResource>(partnerships));
        //}

        //[HttpGet]
        //[SwaggerResponse("200", typeof(List<PartnershipResource>), Description = "OK Result")]
        //[Route("partnerships")]
        //public async Task<IActionResult> Partnerships()
        //{
        //    IList<PartnershipResource> partnerships = await _mediator.Send(new GetLogisticsPartnersListQuery
        //    {
        //        OrganisationId = this.OrganisationId,
        //        OrganisationType = this.OrganisationType,
        //    });
        //    return new OkObjectResult(partnerships);
        //}

    }
}