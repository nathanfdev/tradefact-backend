using Core.Common;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
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
using Tradefact.Api.Model;
using Tradefact.Application.Models;
using Tradefact.Application.Network;
using Tradefact.Data;
using Tradfact.Api.Requests;
using Tradfact.Api.Responses;
using X.PagedList;

namespace Tradefact.Api.Controllers
{

    public abstract class DirectoryController : BaseApiController
    {
        public virtual OrganisationTypeEnum OrganisationType => OrganisationTypeEnum.SHIPPER;

        private readonly ITradefactActivityService _tradefactActivityService;
        protected readonly ILocationService _locationService;
        private readonly IMediator _mediator;

        public DirectoryController(TradefactDbContext context) : base(context)
        {
        }

        public DirectoryController(TradefactDbContext context, IMediator mediator, ITradefactActivityService tradefactActivityService, ILocationService locationService) : base(context)
        {
            _mediator = mediator;
            _tradefactActivityService = tradefactActivityService;
            _locationService = locationService;
        }

        [HttpGet()]
        [SwaggerResponse("200", typeof(ListResource<DirectoryResource>), Description = "OK Result")]
        [Route("")]
        public async Task<IActionResult> List(
            [FromQuery] PagedResultParameters @params, 
            string search = null, 
            bool includeOrders = false, 
            string countryCode = null, 
            ConnectionStatusEnum? status = null,
            SourceEnum? source = null)
        {
            IPagedList<DirectoryResource> network = await _mediator.Send(new GetNetworkListQuery
            {
                OrganisationId = this.OrganisationId,
                Search = search,
                Paging = @params,
                IncludeOrders = includeOrders,
                CountryFilter = countryCode,
                StatusFilter = status,
                SourceFilter = source
            });
            return this.HandleSuccessResponse(new ListResource<DirectoryResource>(network));
        }

        [HttpGet("{id}")]
        [SwaggerResponse("200", typeof(DirectoryResource), Description = "OK Result")]
        public async Task<IActionResult> GetById(string id)
        {
            Organisation org = await GetOrganisation(id);
            if (org == null)
            {
                return new NotFoundResult();
            }
            DirectoryResource retValue = org.Adapt<DirectoryResource>();
            retValue.NetworkType = org.GetNetworkConnectionType(this.OrganisationId);
            return new OkObjectResult(retValue);
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(DirectoryResource), Description = "Created result")]
        public virtual async Task<IActionResult> Create([FromBody]CreateOrganisationRequest request)
        {
            var org = new Organisation { Id = Guid.NewGuid(), ParentId = this.OrganisationId, Name = request.Name, PaymentTerms = 30, OrganisationTypeId = this.OrganisationType, Currency = request.Currency };
            _context.Organisations.Add(org);
            _ = await _context.SaveChangesAsync();

            var eventName = $"{this.OrganisationType.ToString().ToLower()}_added_to_directory";
            await _tradefactActivityService.TrackEvent(User, this.OrganisationId, eventName, new TrackWith { Segment = true, Tradefact = false }, new EventProps { Segment = request });

            return new OkObjectResult(org.Adapt<DirectoryResource>());
        }

        [HttpPut]
        [SwaggerResponse("200", typeof(DirectoryResource), Description = "Updated result")]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromBody]UpdateCompanyRequest request, string id)
        {
            Organisation org = await GetOrganisation(id);
            org = request.Adapt(org);
            _ = await _context.SaveChangesAsync();
            return new OkObjectResult(org.Adapt<DirectoryResource>());
        }

        [HttpPatch]
        [SwaggerResponse("200", typeof(DirectoryResource), Description = "Update result")]
        [Route("{id}")]
        public virtual async Task<IActionResult> Patch([FromBody] JsonPatchDocument<UpdateCompanyRequest> patchDoc, string id)
        {
            if (patchDoc != null)
            {
                Organisation org = await GetOrganisation(id);
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

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            Organisation org = await GetOrganisation(id);
            if (org == null)
            {
                return new NotFoundResult();
            }
            org.IsActive = false;
            _ = await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(NoteResponse), Description = "Updated result")]
        [Route("{id:guid}/notes")]
        public async Task<IActionResult> GetNotes(string id)
        {
            Organisation org = await GetOrganisationWithNotes(id);
            if (org == null || org.Notes == null || !org.Notes.Any())
            {
                return new OkObjectResult(new NoteResponse());
            }
            try
            {
                NoteResponse response = org.Notes[0].Adapt<NoteResponse>();
                return new OkObjectResult(response);
            }
            catch (Exception e)
            {

                throw(e);
            }

        }

        [HttpPut]
        [SwaggerResponse("200", typeof(NoteResponse), Description = "Updated result")]
        [Route("{id:guid}/notes")]
        public async Task<IActionResult> UpdateNotes([FromBody]AddUpdateNoteRequest request, string id)
        {
            Organisation org = await GetOrganisationWithNotes(id);
            if (org == null)
            {
                return new NotFoundResult();
            }
            if (!org.Notes.Any())
            {
                org.Notes.Add(new OrganisationNote());
            }
            OrganisationNote note = org.Notes.FirstOrDefault();
            note.Note = request.Note;
            _ = await _context.SaveChangesAsync();

            return new OkObjectResult(note.Adapt<NoteResponse>());
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<IAddressResource>), Description = "Updated result")]
        [Route("{id:guid}/address")]
        public async Task<IActionResult> GetDirectoryAddresses(string id, [FromQuery] PagedResultParameters @params, string search = null)
        {
            Organisation org = await GetOrganisationWithAddress(id);
            if (org == null)
            {
                return new NotFoundResult();
            }
            IQueryable<Address> query = org.Addresses.Where(q => q.IsActive).AsQueryable();
            if (org.GetNetworkConnectionType(this.OrganisationId) == NetworkConnectionTypeEnum.CONNECTED)
            {
                //  Connected Organisation - Show only those addresses exposed by org & those managed by this organisation
                query = query.Where(q => q.B2BConnectionId == this.OrganisationId || q.B2BConnectionId == null);
            } else
            {
                query = query.Where(q => q.B2BConnectionId == null);
            }
            if (search != null && search.Length > 1)
            {
                query = query.Where(q => EF.Functions.Like(q.Name, $"%{search}%") || EF.Functions.Like(q.AddressLine1, $"%{search}%") || EF.Functions.Like(q.City, $"%{search}%"));
            }
            List<IAddressResource> addresses = new List<IAddressResource>();
            addresses.AddRange(query.Where(q => q.OrganisationId != this.OrganisationId && q.B2BConnectionId == null && org.ParentId == this.OrganisationId).ToList().Adapt<List<ManagedAddressResource>>());
            addresses.AddRange(query.Where(q => q.OrganisationId != this.OrganisationId && q.B2BConnectionId == null && org.ParentId == null).ToList().Adapt<List<OwnedAddressResource>>());
            addresses.AddRange(query.Where(q => q.B2BConnectionId == this.OrganisationId).ToList().Adapt<List<ConnectedAddressResource>>());



            //if (search != null && search.Length > 1)
            //{
            //    return new OkObjectResult(new ListResource<AddressResource>(addresses.OrderBy(x => x.Position(search)).ThenBy(x => x.Name).ToPagedList(@params.PageNumber, @params.PageSize)));
            //}
            return new OkObjectResult(new ListResource<IAddressResource>(addresses.ToPagedList(@params.PageNumber, @params.PageSize)));
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(AddressResource), Description = "Updated result")]
        [Route("{id:guid}/address/{addressid:guid}")]
        public async Task<IActionResult> GetDirectoryAddressById(string id, string addressid)
        {
            Organisation org = await GetOrganisationWithAddress(id);
            if (org == null || org.Addresses == null || !org.Addresses.Any(q => q.Id == new Guid(addressid)))
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(org.Addresses.Single(q => q.Id == new Guid(addressid)).Adapt<AddressResource>());
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(AddressResource), Description = "Created result")]
        [Route("{id:guid}/address")]
        public async Task<IActionResult> CreateAddress([FromBody]CreateAddressRequest request, string id)
        {
            Organisation org = await GetOrganisationWithAddress(id);
            if (org == null)
            {
                return new NotFoundResult();
            }
            Address add = request.Adapt<Address>();
            add.Id = new Guid();
            add.Position = await _locationService.GetAddressLocation(add);
            if (org.GetNetworkConnectionType(this.OrganisationId) == NetworkConnectionTypeEnum.CONNECTED)
            {
                add.B2BConnectionId = this.OrganisationId;
            }
            org.Addresses.Add(add);
            _ = await _context.SaveChangesAsync();

            await _tradefactActivityService.TrackEvent(User, this.OrganisationId, "location_added", new TrackWith { Segment = true, Tradefact = false }, new EventProps { Segment = request });

            return new OkObjectResult(add.Adapt<AddressResource>());
        }

        [HttpPut]
        [SwaggerResponse("200", typeof(AddressResource), Description = "Updated result")]
        [Route("{id:guid}/address/{addressid:guid}")]
        public async Task<IActionResult> UpdateAddress([FromBody]UpdateAddressRequest request, string id, string addressid)
        {
            Organisation org = await GetOrganisation(id);
            if (org == null)
            {
                return new NotFoundResult();
            }
            Address add;
            if (org.GetNetworkConnectionType(this.OrganisationId) == NetworkConnectionTypeEnum.CONNECTED)
            {
                add = await _context.Addresses.SingleOrDefaultAsync(q => q.Id == new Guid(addressid) && q.B2BConnectionId == this.OrganisationId);
            } else
            {
                add = await _context.Addresses.SingleOrDefaultAsync(q => q.Id == new Guid(addressid) && q.OrganisationId == new Guid(id) && q.Organisation.ParentId == this.OrganisationId);
            }

            add = request.Adapt(add);
            add.Position = await _locationService.GetAddressLocation(add);

            _ = await _context.SaveChangesAsync();

            add = await _context.Addresses.Include(i=>i.Country).SingleOrDefaultAsync(q => q.Id == new Guid(addressid));
            return new OkObjectResult(add.Adapt<AddressResource>());
        }

        [HttpPatch]
        [SwaggerResponse("200", typeof(AddressResource), Description = "Updated result")]
        [Route("{id:guid}/address/{addressid:guid}")]
        public async Task<IActionResult> UpdateAddress([FromBody] JsonPatchDocument<UpdateAddressRequest> patchDoc, string id, string addressid)
        {

            if (patchDoc != null)
            {
                Organisation org = await GetOrganisation(id);
                if (org == null)
                {
                    return new NotFoundResult();
                }
                Address add;
                if (org.GetNetworkConnectionType(this.OrganisationId) == NetworkConnectionTypeEnum.CONNECTED)
                {
                    add = await _context.Addresses.SingleOrDefaultAsync(q => q.Id == new Guid(addressid) && q.B2BConnectionId == this.OrganisationId);
                }
                else
                {
                    add = await _context.Addresses.SingleOrDefaultAsync(q => q.Id == new Guid(addressid) && q.OrganisationId == new Guid(id) && q.Organisation.ParentId == this.OrganisationId);
                }
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
        [Route("{id:guid}/address/{addressid:guid}")]
        public async Task<IActionResult> DeleteAddress(string id, string addressid)
        {
            Organisation org = await GetOrganisation(id);
            if (org == null)
            {
                return new NotFoundResult();
            }
            Address add;
            if (org.GetNetworkConnectionType(this.OrganisationId) == NetworkConnectionTypeEnum.CONNECTED)
            {
                add = await _context.Addresses.SingleOrDefaultAsync(q => q.Id == new Guid(addressid) && q.B2BConnectionId == this.OrganisationId);
            }
            else
            {
                add = await _context.Addresses.SingleOrDefaultAsync(q => q.Id == new Guid(addressid) && q.OrganisationId == new Guid(id) && q.Organisation.ParentId == this.OrganisationId);
            }
            add.IsActive = false;
            _ = await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpGet]
        [SwaggerResponse("200", typeof(ListResource<ContactResource>), Description = "Updated result")]
        [Route("{id:guid}/Contact")]
        public async Task<IActionResult> GetDirectoryContacts(string id, [FromQuery] PagedResultParameters @params, string search = null, string locationid = null)
        {
            Organisation org = await GetOrganisationWithContacts(id);
            if (org == null)
            {
                List<ContactResource> notfound = new List<ContactResource>();
                return new OkObjectResult(new ListResource<ContactResource>(notfound.AsQueryable().ToPagedList(@params.PageNumber, @params.PageSize)));
            }

            if (org.GetNetworkConnectionType(this.OrganisationId) == NetworkConnectionTypeEnum.MANAGED || org.GetNetworkConnectionType(this.OrganisationId) == NetworkConnectionTypeEnum.SELF)
            {
                IQueryable<ContactResource> query = org.Contacts.Where(q => q.IsActive).Adapt<List<ContactResource>>().AsQueryable();
                if (search != null && search.Length > 1)
                {
                    query = query.Where(q => q.FullName.Contains(search, StringComparison.InvariantCultureIgnoreCase));
                }
                if (locationid != null && locationid.Length > 1)
                {
                    query = query.Where(q => q.LocationId == new Guid(locationid));
                }
                IPagedList<ContactResource> contacts = query.Adapt<List<ManagedContactResource>>().ToPagedList(@params.PageNumber, @params.PageSize);
                foreach (ContactResource c in contacts)
                {
                    if (c.LocationId.HasValue)
                    {
                        Address a = await _context.Addresses.SingleOrDefaultAsync(q => q.Id == c.LocationId);
                        if (a != null && a.Name != null) c.LocationName = a.Name;
                    }
                }
                return new OkObjectResult(new ListResource<ContactResource>(contacts));
            } else
            {
                B2BConnection connection = await _context.B2BConnections
                    .Include(i => i.Contacts).ThenInclude(i => i.Email)
                    .Include(i => i.Contacts).ThenInclude(i => i.Phone)
                    .FirstOrDefaultAsync(q => q.OrgansationId == this.OrganisationId && q.LinkedOrganisationId == org.Id);

                // Managed contacts are associated with the connection...
                IQueryable<ContactResource> contactQuery = connection.Contacts
                    .Where(q => q.IsActive).ToList()
                    .Adapt<List<ContactResource>>()
                    .AsQueryable();

                // ... while connected contacts come from linked organisations users
                IQueryable<ContactResource> userQuery = _context.Users
                    .Where(q => 
                        q.IsExposedToOtherOrganization &&
                        q.OrganisationId == connection.LinkedOrganisationId && 
                        q.Status == UserStatus.Active).ToList()
                    .Adapt<List<ContactResource>>()
                    .AsQueryable();

                List<ContactResource> contacts = new List<ContactResource>();
                contacts.AddRange(contactQuery.Adapt<List<ManagedContactResource>>().ToList());
                contacts.AddRange(userQuery.Adapt<List<ConnectedContactResource>>().ToList());

                if (search != null && search.Length > 1)
                {
                    contacts = contacts.Where(q => q.FullName.Contains(search, StringComparison.InvariantCultureIgnoreCase)).ToList();
                }
                if (locationid != null && locationid.Length > 1)
                {
                    contacts = contacts.Where(q => q.LocationId == new Guid(locationid)).ToList();
                }

                IPagedList<ContactResource> pagedList = contacts.ToPagedList(@params.PageNumber, @params.PageSize);
                foreach (ContactResource c in pagedList)
                {
                    if (c.LocationId.HasValue)
                    {
                        Address a = await _context.Addresses.SingleOrDefaultAsync(q => q.Id == c.LocationId);
                        if (a != null && a.Name != null) c.LocationName = a.Name;
                    }
                }

                return new OkObjectResult(new ListResource<ContactResource>(pagedList));
            }
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(ContactResource), Description = "Updated result")]
        [Route("{id:guid}/Contact/{Contactid:guid}")]
        public async Task<IActionResult> GetDirectoryContactById(string id, string contactid)
        {
            Organisation org = await GetOrganisationWithContacts(id);
            if (org == null || org.Contacts == null || !org.Contacts.Any(q => q.Id == new Guid(contactid)))
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(org.Contacts.Single(q => q.Id == new Guid(contactid)).Adapt<DirectoryResource>());
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(ContactResource), Description = "Created result")]
        [Route("{id:guid}/Contact")]
        public async Task<IActionResult> CreateContact([FromBody]CreateContactRequest request, string id)
        {
            Organisation org = await GetOrganisation(id);
            if (org == null)
            {
                return new NotFoundResult();
            }

            if (org.GetNetworkConnectionType(this.OrganisationId) == NetworkConnectionTypeEnum.MANAGED || org.GetNetworkConnectionType(this.OrganisationId) == NetworkConnectionTypeEnum.SELF)
            {
                OrganisationContact add = request.Adapt<OrganisationContact>();
                if (request.Email != null && request.Email.Email != null)
                {
                    add.Email = new List<OrganisationContactEmailAddress>();
                    OrganisationContactEmailAddress email = request.Email.Adapt<OrganisationContactEmailAddress>();
                    add.Email.Add(email);
                }
                if (request.Phone != null && request.Phone.Number != null)
                {
                    add.Phone = new List<OrganisationContactPhone>();
                    OrganisationContactPhone ph = request.Phone.Adapt<OrganisationContactPhone>();
                    add.Phone.Add(ph);
                }
                add.Id = new Guid();
                org.Contacts.Add(add);

                _ = await _context.SaveChangesAsync();
                return new OkObjectResult(add.Adapt<ContactResource>());
            } else
            {
                B2BConnection connection = await _context.B2BConnections.FirstOrDefaultAsync(q => q.OrgansationId == this.OrganisationId && q.LinkedOrganisationId == org.Id);
                ConnectionContact contact_to_add = request.Adapt<ConnectionContact>();

                if (request.Email != null && request.Email.Email != null)
                {
                    contact_to_add.Email = new List<ConnectionContactEmailAddress>();
                    ConnectionContactEmailAddress email = request.Email.Adapt<ConnectionContactEmailAddress>();
                    contact_to_add.Email.Add(email);
                }
                if (request.Phone != null && request.Phone.Number != null)
                {
                    contact_to_add.Phone = new List<ConnectionContactPhone>();
                    ConnectionContactPhone ph = request.Phone.Adapt<ConnectionContactPhone>();
                    contact_to_add.Phone.Add(ph);
                }
                contact_to_add.Id = new Guid();
                connection.Contacts.Add(contact_to_add);

                _ = await _context.SaveChangesAsync();
                return new OkObjectResult(contact_to_add.Adapt<ContactResource>());
            }

        }

        [HttpPut]
        [SwaggerResponse("200", typeof(ContactResource), Description = "Updated result")]
        [Route("{id:guid}/Contact/{contactid:guid}")]
        public async Task<IActionResult> UpdateContact([FromBody]UpdateContactRequest request, string id, string contactid)
        {
            Organisation org = await GetOrganisation(id);
            if (org == null)
            {
                return new NotFoundResult();
            }
            if (org.GetNetworkConnectionType(this.OrganisationId) == NetworkConnectionTypeEnum.MANAGED || org.GetNetworkConnectionType(this.OrganisationId) == NetworkConnectionTypeEnum.SELF)
            {
                org = await GetOrganisationWithContacts(id);
                if (org == null || org.Contacts == null || !org.Contacts.Any(q => q.Id == new Guid(contactid)))
                {
                    return new NotFoundResult();
                }

                OrganisationContact add = org.Contacts.FirstOrDefault(q => q.Id == new Guid(contactid));
                add.FullName = request.FullName;

                var updatedEmail = new OrganisationContactEmailAddress
                {
                    Email = request.Email.Email,
                    IsDefault = request.Email.IsDefault
                };
                add.Email = new List<OrganisationContactEmailAddress> { updatedEmail };

                var updatedPhone = new OrganisationContactPhone
                {
                    Number = request.Phone.Number,
                    AreaCode = request.Phone.AreaCode,
                    CountryCode = request.Phone.CountryCode,
                    IsDefault = request.Phone.IsDefault
                };
                add.Phone = new List<OrganisationContactPhone> { updatedPhone };

                add.Department = request.Department;
                add.LocationId = request.LocationId;

                _ = await _context.SaveChangesAsync();

                return new OkObjectResult(add.Adapt<ContactResource>());
            }
            else
            {
                B2BConnection connection = await _context.B2BConnections
                    .Include(i => i.Contacts).ThenInclude(i => i.Email)
                    .Include(i => i.Contacts).ThenInclude(i => i.Phone)
                    .FirstOrDefaultAsync(q => q.OrgansationId == this.OrganisationId && q.LinkedOrganisationId == org.Id);

                if (connection == null || connection.Contacts == null || !connection.Contacts.Any(q => q.Id == new Guid(contactid)))
                {
                    return new NotFoundResult();
                }
                ConnectionContact c = connection.Contacts.FirstOrDefault(q => q.Id == new Guid(contactid));
                c.FullName = request.FullName;

                c.Email[0].Email = request.Email.Email;
                c.Email[0].IsDefault = true;

                c.Phone[0].Number = request.Phone.Number;
                c.Phone[0].AreaCode = request.Phone.AreaCode;
                c.Phone[0].CountryCode = request.Phone.CountryCode;
                c.Phone[0].IsDefault = true;

                c.Department = request.Department;
                c.LocationId = request.LocationId;

                _ = await _context.SaveChangesAsync();

                return new OkObjectResult(c.Adapt<ContactResource>());
            }

        }

        [HttpPatch]
        [SwaggerResponse("200", typeof(ContactResource), Description = "Updated result")]
        [Route("{id:guid}/Contact/{Contactid:guid}")]
        public async Task<IActionResult> UpdateContact([FromBody] JsonPatchDocument<UpdateContactRequest> patchDoc, string id, string contactid)
        {

            if (patchDoc != null)
            {
                Organisation org = await GetOrganisationWithContacts(id);
                if (org == null || org.Contacts == null || !org.Contacts.Any(q => q.Id == new Guid(contactid)))
                {
                    return new NotFoundResult();
                }
                Contact add = org.Contacts.Single(q => q.Id == new Guid(contactid));
                UpdateContactRequest request = new UpdateContactRequest();
                request = org.Adapt(request);
                patchDoc.ApplyTo(request, ModelState);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                add = request.Adapt(add);
                _ = await _context.SaveChangesAsync();

                return new ObjectResult(add.Adapt<ContactResource>());
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpDelete]
        [Route("{id:guid}/Contact/{Contactid:guid}")]
        public async Task<IActionResult> DeleteContact(string id, string contactid)
        {
            Organisation org = await GetOrganisation(id);
            if (org == null)
            {
                return new NotFoundResult();
            }
            if (org.GetNetworkConnectionType(this.OrganisationId) == NetworkConnectionTypeEnum.MANAGED || org.GetNetworkConnectionType(this.OrganisationId) == NetworkConnectionTypeEnum.SELF)
            {
                org = await GetOrganisationWithContacts(id);
                if (org == null || org.Contacts == null || !org.Contacts.Any(q => q.Id == new Guid(contactid)))
                {
                    return new NotFoundResult();
                }
                Contact contact = org.Contacts.Single(q => q.Id == new Guid(contactid));
                contact.IsActive = false;
                _ = await _context.SaveChangesAsync();
                return Ok();

            }
            else
            {
                B2BConnection connection = await _context.B2BConnections
                    .Include(i => i.Contacts).ThenInclude(i => i.Email)
                    .Include(i => i.Contacts).ThenInclude(i => i.Phone)
                    .FirstOrDefaultAsync(q => q.OrgansationId == this.OrganisationId && q.LinkedOrganisationId == org.Id);

                if (connection == null || connection.Contacts == null || !connection.Contacts.Any(q => q.Id == new Guid(contactid)))
                {
                    return new NotFoundResult();
                }
                ConnectionContact c = connection.Contacts.FirstOrDefault(q => q.Id == new Guid(contactid));
                c.IsActive = false;
                _ = await _context.SaveChangesAsync();
                return new OkObjectResult(c.Adapt<ContactResource>());
            }



        }

        private async Task<Organisation> GetOrganisation(string id)
        {
            Organisation org = await _context.Organisations.SingleOrDefaultAsync(q => q.Id == new Guid(id));
            if (org == null)
            {
                return null;
            }
            return org;
        }

        private async Task<Organisation> GetOrganisationWithNotes(string id)
        {
            Organisation org = await _context.Organisations.Include(i => i.Notes).SingleOrDefaultAsync(q =>  q.Id == new Guid(id));
            if (org == null)
            {
                return null;
            }
            return org;
        }

        private async Task<Organisation> GetOrganisationWithAddress(string id)
        {
            Organisation org = await _context.Organisations.Include(i => i.Addresses).ThenInclude(address => address.Country).SingleOrDefaultAsync(q => q.Id == new Guid(id));
            if (org == null)
            {
                return null;
            }
            return org;
        }

        private async Task<Organisation> GetOrganisationWithContacts(string id)
        {
            Organisation org = await _context.Organisations.Include(i => i.Contacts).ThenInclude(contact => contact.Email).SingleOrDefaultAsync(q => q.Id == new Guid(id));
            if (org == null)
            {
                return null;
            }
            return org;
        }

    }
}