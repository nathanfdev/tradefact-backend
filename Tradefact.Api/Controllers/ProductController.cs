using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Common;
using Core.Dtos.PurchaseOrder;
using Core.Interfaces;
using Core.Models;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NSwag.Annotations;
using Tradefact.Api.Model;
using Tradefact.Application.Models;
using Tradefact.Application.Products.Queries;
using Tradefact.Application.PurchaseOrders.Commands;
using Tradefact.Application.PurchaseOrders.Queries;
using Tradefact.Data;
using Tradfact.Api.Requests;
using X.PagedList;

namespace Tradefact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly ITradefactActivityService _tradefactActivityService;

        public ProductController(TradefactDbContext context, IMediator mediator, ITradefactActivityService tradefactActivityService) : base(context)
        {
            _mediator = mediator;
            _tradefactActivityService = tradefactActivityService;
        }

        private async Task<ProductResource> ByIdAsync(Guid id, bool includeSuppliers = default)
        {
            Product product = await _context.Products.SingleOrDefaultAsync(q => q.CompanyId == this.OrganisationId && q.IsActive && q.Id == id);
            if (product != null && includeSuppliers)
            {
                product.Suppliers = await _context.ProductSuppliers.Include(ps => ps.Currencies).Where(q => q.ProductId == product.Id && q.IsActive).Include(x => x.Supplier).ToListAsync();
                foreach (var ps in product.Suppliers)
                {
                    ps.Currencies = ps.Currencies.Where(c => c.IsActive).ToList();
                }
            }
            return product.Adapt<ProductResource>();
        }


        // GET: api/Product
        [HttpGet()]
        [SwaggerResponse("200", typeof(ListResource<ProductResource>), Description = "OK Result")]
        [Route("")]
        public async Task<IActionResult> Get([FromQuery] PagedResultParameters @params, string sortBy, string search = null)
        {

            IPagedList<ProductResource> products = await _mediator.Send(new GetProductOrderQuantityListQuery
            {
                OrganisationId = this.OrganisationId,
                Search = search,
                Paging = @params,
                SortBy = sortBy
            });
            if (products == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(new ListResource<ProductResource>(products));
        }
        
        // GET: api/Product/5
        [HttpGet("{id}", Name = "Get")]
        [SwaggerResponse("200", typeof(ProductResource), Description = "OK Result")]
        public async Task<IActionResult> Get(Guid id, [FromServices] IUserResolverService userResolver)
        {
            ProductResource product = await this.ByIdAsync(id, true);
            if (product == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(product);
        }

        // POST: api/Product
        [HttpPost]
        [SwaggerResponse("200", typeof(ProductResource), Description = "Created result")]
        public async Task<IActionResult> Post([FromBody] CreateProductRequest request)
        {
            Product p = new Product
            {
                CompanyId = this.OrganisationId,
            };
            p = request.Adapt(p);
            p.Suppliers = null;
            _context.Products.Add(p);
            _ = await _context.SaveChangesAsync();

            if (request.Suppliers != null)
            {
                foreach (var item in request.Suppliers)
                {
                    ProductSupplier ps = new ProductSupplier()
                    {
                        Id = Guid.NewGuid(),
                        ProductId = p.Id,
                        SupplierId = item.SupplierId,
                        IsActive = true,
                        SupplierReference = item.SupplierReference,
                        Price = item.Price,
                        OrderQuantityMinimum = item.OrderQuantityMinimum,
                    };

                    if (item.Currencies.Count > 0)
                    {
                        ps.Currencies = new List<ProductSupplierCurrency>();
                        foreach (var currency in item.Currencies)
                        {
                            // ps.Currencies.Add(currency);
                            ps.Currencies.Add(
                                new ProductSupplierCurrency
                                {
                                    Id = Guid.NewGuid(),
                                    ProductSupplierId = ps.Id,
                                    CurrencyCode = currency.CurrencyCode,
                                    Price = currency.Price
                                }
                            );
                        }
                    }

                    _context.ProductSuppliers.Add(ps);
                }
                _ = await _context.SaveChangesAsync();
            }

            await _tradefactActivityService.TrackEvent(
                User, 
                this.OrganisationId, 
                "product_added_to_catalogue", 
                new TrackWith { Segment = true, Tradefact = false },
                new EventProps { Segment = request }
            );

            return new OkObjectResult(await this.ByIdAsync(p.Id, true));
        }

        // POST: api/Product/FromInbox/{id}
        [HttpPost]
        [SwaggerResponse("200", typeof(List<ProductResource>), Description = "Created result")]
        [Route("FromInbox/{id:guid}")]
        public async Task<IActionResult> CreateFromInbox(string id)
        {
            List<InboxPurchaseOrderLineItemResource> items = await _mediator.Send(new GetInboxPurchaseOrderLineItemsQuery
            {
                OrganisationId = this.OrganisationId,
                ExternalPurchaseOrderId = new Guid(id)
            });

            var response = new List<ProductResource>();

            foreach (var skuToCreate in items.Where(q => !q.ExistsInCatalogue))
            {
                Product p = new Product
                {
                    Id = Guid.NewGuid(),
                    CompanyId = this.OrganisationId,
                    SKU = skuToCreate.SKU,
                    Name = skuToCreate.Description,
                    Tags = skuToCreate.Tags,
                    // TODO - decide on a better way to define defaults
                    Dimensions = new ImportProductDimensions 
                    {
                        Height = 25,
                        Length = 25,
                        Width = 25,
                        Scale = "CM",
                        Weight = 1.32m,
                        weightMeasurement = "KG"
                    },
                    UnitsPerPackage = 5,
                    Packing = "boxes_cartons",
                    DataComplete = true
                };

                _context.Products.Add(p);
                response.Add(p.Adapt<ProductResource>());
            }

            _ = _context.SaveChangesAsync();

            return new OkObjectResult(response);
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<ProductResource>), Description = "OK Result")]
        [Route("StockLevelProducts")]
        public async Task<IActionResult> GetStockLevelProducts()
        {
            List<Product> products = await _context.Products.Where(x => x.CompanyId == this.OrganisationId).Include(x => x.Suppliers).ToListAsync();
            return new OkObjectResult(products.Adapt<List<ProductResource>>().ToList());
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<ProductResource>), Description = "OK Result")]
        [Route("SupplierProducts")]
        public async Task<IActionResult> GetSupplierProducts([FromQuery] PagedResultParameters @params, Guid? id, string search = null)
        {
            IPagedList<ProductResource> result = await _mediator.Send(new GetProductOrderQuantityListQuery
            {
                OrganisationId = this.OrganisationId,
                Search = search,
                Paging = @params,
                SupplierId = id
            });
            return this.HandleSuccessResponse(new ListResource<ProductResource>(result));
        }

        [HttpPost]
        [Route("ReorderProducts")]
        public async Task<IActionResult> ReorderProducts([FromBody] ReorderProductsRequest request)
        {
            List<string> orderNumbers = new List<string>();
            foreach (var supplier in request.ProductsToOrder)
            {
                Guid id = await _mediator.Send(new PurchaseOrderCreateCommand(this.OrganisationId, supplier.SupplierId, request.CurrencyId));

                PurchaseOrderEditDto edit = new PurchaseOrderEditDto() { Reference = request.OrderReference, SupplierId = supplier.SupplierId };
                _ = await _mediator.Send(new PurchaseOrderUpdateCommand(this.OrganisationId, id, edit));

                List<PurchaseOrderItemEditDto> lines = new List<PurchaseOrderItemEditDto>();
                foreach (var product in supplier.OrderInfo)
                {
                    PurchaseOrderItemEditDto editDto = new PurchaseOrderItemEditDto() { ProductId = product.ProductId,  OrderQuantity = product.OrderQuantity };
                    lines.Add(editDto);
                }
                PurchaseOrderItemCreateCommand cmd = new PurchaseOrderItemCreateCommand(this.OrganisationId, id, lines);
                List<Guid> newItemIds = await _mediator.Send(cmd);

                orderNumbers.Add(_context.PurchaseOrders.First(x => x.Id == id).PurchaseOrderNumber);
            }
            return new OkObjectResult(orderNumbers);
        }

        // PUT: api/Product/5
        [HttpPut]
        [SwaggerResponse("200", typeof(ProductResource), Description = "Created result")]
        public async Task<IActionResult> Put(string id, [FromBody] UpdateProductRequest request, [FromServices] IUserResolverService userResolver)
        {

            Product product = await _context.Products.SingleOrDefaultAsync(q => q.CompanyId == this.OrganisationId && q.Id == new Guid(id));
            if (product == null)
            {
                return new NotFoundResult();
            }
            product = request.Adapt(product);

            if (request.Tags == null)
            {
                product.Tags = null;
            }

            List<ProductSupplier> existing_product_suppliers = await _context.ProductSuppliers.Include(ps => ps.Currencies).Where(q => q.ProductId == product.Id && q.IsActive).ToListAsync();
            List<ProductSupplierCurrency> existing_product_supplier_currencies = await _context.ProductSupplierCurrency.Where(q => q.IsActive).ToListAsync();
            foreach (var ps in existing_product_suppliers)
            {
                if (!request.Suppliers.Any(s => s.SupplierId == ps.SupplierId))
                {
                    ps.IsActive = false;
                }
            }

            foreach (var psc in existing_product_supplier_currencies)
            {
                var selectedProductSupplier = existing_product_suppliers.Find(ps => ps.Id == psc.ProductSupplierId);
                if (selectedProductSupplier != null && !request.Suppliers.Any(x => x.Currencies.Any(c => c.CurrencyCode == psc.CurrencyCode) && x.SupplierId == selectedProductSupplier.SupplierId))
                {
                    psc.IsActive = false;
                }
            }

            // Create if not exists 
            foreach (var ps in request.Suppliers)
            {
                if (!existing_product_suppliers.Any(x => x.SupplierId == ps.SupplierId))
                {
                    ProductSupplier new_product_supplier = new ProductSupplier
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        SupplierId = ps.SupplierId,
                        IsActive = true,
                        SupplierReference = ps.SupplierReference,
                        Price = ps.Price,
                        OrderQuantityMinimum = ps.OrderQuantityMinimum
                    };

                    if (ps.Currencies != null && ps.Currencies.Count > 0)
                    {
                        new_product_supplier.Currencies = new List<ProductSupplierCurrency>();
                        foreach (var currency in ps.Currencies)
                        {
                            new_product_supplier.Currencies.Add(
                                new ProductSupplierCurrency
                                {
                                    Id = Guid.NewGuid(),
                                    ProductSupplierId = new_product_supplier.Id,
                                    CurrencyCode = currency.CurrencyCode,
                                    Price = currency.Price
                                }
                            );
                        }
                    }
                    _context.ProductSuppliers.Add(new_product_supplier);
                }

                foreach (var existing_ps in existing_product_suppliers.Where(q=>q.SupplierId == ps.SupplierId))
                {
                    existing_ps.IsActive = true;
                    existing_ps.SupplierReference = ps.SupplierReference;
                    existing_ps.Price = ps.Price;
                    existing_ps.OrderQuantityMinimum = ps.OrderQuantityMinimum;

                    if (ps.Currencies != null && ps.Currencies.Count > 0)
                    {
                        foreach (var currency in ps.Currencies)
                        {
                            if (!existing_product_supplier_currencies.Any(x => x.CurrencyCode == currency.CurrencyCode && x.ProductSupplierId == existing_ps.Id))
                            {
                                existing_ps.Currencies.Add(currency);
                            }
                        }
                    }
                }
            }

            _context.Entry(product).State = EntityState.Modified;
            _ = await _context.SaveChangesAsync();

            await _tradefactActivityService.TrackEvent(
                User,
                this.OrganisationId,
                "product_info_updated",
                new TrackWith { Segment = false, Tradefact = true },
                new EventProps { Tradefact = new TradefactEventProps { Reference = id, Type = ActivityTypeEnum.INFO, Entity = ActivityEntityTypeEnum.PRODUCT } }
            );

            return new OkObjectResult(product.Adapt<ProductResource>());
        }


        // PUT: api/Product/5
        [HttpPatch]
        [SwaggerResponse("200", typeof(ProductResource), Description = "Created result")]
        public async Task<IActionResult> Patch(string id, [FromBody] JsonPatchDocument<UpdateProductRequest> patchDoc)
        {
            if (patchDoc != null)
            {
                Product product = await _context.Products.SingleOrDefaultAsync(q => q.CompanyId == this.OrganisationId && q.Id == new Guid(id));
                UpdateProductRequest request = new UpdateProductRequest();
                request = product.Adapt(request);
                patchDoc.ApplyTo(request, ModelState);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                product = request.Adapt(product);
                _ = await _context.SaveChangesAsync();
                return new ObjectResult(product.Adapt<ProductResource>());
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            Product product = await _context.Products.SingleOrDefaultAsync(q => q.CompanyId == this.OrganisationId && q.Id == new Guid(id) && q.IsActive);
            if (product == null)
            {
                return new NotFoundResult();
            }
            product.IsActive = false;
            _ = await _context.SaveChangesAsync();
            return Ok();
        }

        private async Task<Activity> CreateProductActivity(Guid id, ActivityTypeEnum type, string description)
        {
            var product = await _context.Products.FindAsync(id);
            var productObjectToSerialise = product.Adapt<ProductResource>();
            var serialisedData = JsonConvert.SerializeObject(productObjectToSerialise);

            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            string userId = identity.FindFirst(q => q.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            string userFullName = identity.Claims.Where(c => c.Type == "name").Select(c => c.Value).SingleOrDefault();

            var activity = new Activity
            {
                UserId = new Guid(userId),
                OrganisationId = this.OrganisationId,
                Type = type,
                Text1 = product.Name,
                Text2 = userFullName,
                Description = description,
                Entity = ActivityEntityTypeEnum.PRODUCT,
                Data = serialisedData
            };

            return activity;
        }
    }
}
