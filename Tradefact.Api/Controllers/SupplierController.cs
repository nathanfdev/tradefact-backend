using Core.Common;
using Core.Enums;
using Core.Models;
using Core.Models.Tracking;
using Mapster;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.Api.Model;
using Tradefact.Application.Models;
using Tradefact.Data;
using Tradfact.Api.Requests;
using X.PagedList;

namespace Tradefact.Api.Controllers
{
    public partial class SupplierController : DirectoryController
    {
        public override OrganisationTypeEnum OrganisationType => OrganisationTypeEnum.SUPPLIER;
        public SupplierController(TradefactDbContext context) : base(context) { }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<ProductVariantResource>), Description = "Updated result")]
        [Route("{id:guid}/productvariant")]
        public async Task<IActionResult> GetProductVariants(string id, [FromQuery] PagedResultParameters @params, string search = null)
        {
 
            Organisation org = await _context.Organisations.Include(i => i.ProductVariants).ThenInclude(v => v.Product).SingleOrDefaultAsync(q => q.ParentId == this.OrganisationId && q.OrganisationTypeId == this.OrganisationType && q.Id == new Guid(id));
            if (org == null)
            {
                return new NotFoundResult();
            }
            try
            {
                IQueryable<ProductVariant> variants_query = org.ProductVariants.Where(q => q.IsActive).AsQueryable();
                if (search != null && search.Length > 0)
                {
                    variants_query = variants_query.Where(q => EF.Functions.Like(q.Name, $"%{search}%") || EF.Functions.Like(q.Nickname, $"%{search}%") || EF.Functions.Like(q.SKU, $"%{search}%"));
                    IQueryable<SortedProductVariantResource> sorted = variants_query.Adapt<List<SortedProductVariantResource>>().AsQueryable();
                    return new OkObjectResult(new ListResource<ProductVariantResource>(sorted.OrderBy(x => x.Position(search)).ThenBy(x => x.Name).ToPagedList(@params.PageNumber, @params.PageSize)));
                }
                IQueryable<ProductVariantResource> query = variants_query.Adapt<List<ProductVariantResource>>().AsQueryable();
                return new OkObjectResult(new ListResource<ProductVariantResource>(query.ToPagedList(@params.PageNumber, @params.PageSize)));
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ProductVariantResource), Description = "Updated result")]
        [Route("{id:guid}/productvariant/{variantid:guid}")]
        public async Task<IActionResult> GetProductVariantById(string id, string variantid)
        {
            Organisation org = await _context.Organisations.Include(i => i.ProductVariants).ThenInclude(v => v.Product).SingleOrDefaultAsync(q => q.ParentId == this.OrganisationId && q.OrganisationTypeId == this.OrganisationType && q.Id == new Guid(id));
            if (org == null || org.ProductVariants == null || !org.ProductVariants.Any(q => q.Id == new Guid(variantid)))
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(org.ProductVariants.Single(q => q.Id == new Guid(variantid)).Adapt<ProductVariantResource>());
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(ProductVariantResource), Description = "Created result")]
        [Route("{id:guid}/productvariant")]
        public async Task<IActionResult> CreateProductVariant([FromBody]CreateProductVariantRequest request, string id)
        {
            Organisation org = await _context.Organisations.SingleAsync(q => q.ParentId == this.OrganisationId && q.OrganisationTypeId == this.OrganisationType && q.Id == new Guid(id));
            if (org == null)
            {
                return new NotFoundResult();
            }
            ProductVariant v = request.Adapt<ProductVariant>();
            v.Id = new Guid();

            org.ProductVariants.Add(v);

            _ = await _context.SaveChangesAsync();
            return new OkObjectResult(v.Adapt<ProductVariantResource>());
        }

        [HttpPut]
        [SwaggerResponse("200", typeof(ProductVariantResource), Description = "Updated result")]
        [Route("{id:guid}/productvariant/{variantid:guid}")]
        public async Task<IActionResult> UpdateProductVariant([FromBody]UpdateProductVariantRequest request, string id, string variantid)
        {
            ProductVariant v = await _context.ProductVariants.SingleOrDefaultAsync(q => q.Id == new Guid(variantid) && q.SupplierId == new Guid(id) && q.Supplier.ParentId == this.OrganisationId);
            if (v == null)
            {
                return new NotFoundResult();
            }
            v = request.Adapt(v);
            _ = await _context.SaveChangesAsync();

            v = await _context.ProductVariants.Include(i => i.Product).SingleOrDefaultAsync(q => q.Id == new Guid(variantid));
            return new OkObjectResult(v.Adapt<ProductVariantResource>());
        }

        [HttpPatch]
        [SwaggerResponse("200", typeof(AddressResource), Description = "Updated result")]
        [Route("{id:guid}/productvariant/{variantid:guid}")]
        public async Task<IActionResult> UpdateProductVariant([FromBody] JsonPatchDocument<UpdateProductVariantRequest> patchDoc, string id, string variantid)
        {

            if (patchDoc != null)
            {
                Organisation org = await _context.Organisations.Include(i => i.ProductVariants).SingleOrDefaultAsync(q => q.ParentId == this.OrganisationId && q.OrganisationTypeId == this.OrganisationType && q.Id == new Guid(id));
                if (org == null || org.ProductVariants == null || !org.ProductVariants.Any(q => q.Id == new Guid(variantid)))
                {
                    return new NotFoundResult();
                }
                ProductVariant v = org.ProductVariants.Single(q => q.Id == new Guid(variantid));
                UpdateProductVariantRequest request = new UpdateProductVariantRequest();
                request = org.Adapt(request);
                patchDoc.ApplyTo(request, ModelState);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                v = request.Adapt(v);
                _ = await _context.SaveChangesAsync();

                v = await _context.ProductVariants.Include(i => i.Product).SingleOrDefaultAsync(q => q.Id == new Guid(variantid));
                return new OkObjectResult(v.Adapt<ProductVariantResource>());
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpDelete]
        [Route("{id:guid}/productvariant/{variantid:guid}")]
        public async Task<IActionResult> DeleteProductVariant(string id, string variantid)
        {
            Organisation org = await _context.Organisations.Include(i => i.ProductVariants).SingleOrDefaultAsync(q => q.ParentId == this.OrganisationId && q.OrganisationTypeId == this.OrganisationType && q.Id == new Guid(id));
            if (org == null || org.ProductVariants == null || !org.ProductVariants.Any(q => q.Id == new Guid(variantid)))
            {
                return new NotFoundResult();
            }
            ProductVariant v = org.ProductVariants.Single(q => q.Id == new Guid(variantid));
            v.IsActive = false;
            _ = await _context.SaveChangesAsync();
            return Ok();
        }

    }
}