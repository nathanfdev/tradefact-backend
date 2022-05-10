using Core.Interfaces;
using Core.Models;
using Integration.TradefactActivityService.Model;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tradefact.Application.Models;
using Tradefact.Application.Models.Activity;
using Tradefact.Data;

namespace Integration.TradefactActivityService
{
    public class TradefactActivityService : ITradefactActivityService
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly TradefactDbContext _context;

        public TradefactActivityService(IAnalyticsService analyticsService, TradefactDbContext context)
        {
            _analyticsService = analyticsService;
            _context = context;
        }

        public async Task TrackEvent(ClaimsPrincipal user, Organisation org, string eventName, TrackWith analytics, EventProps eventProps)
        {
            if (analytics.Segment)
            {
                await _analyticsService.TrackEvent(user, org, eventName, eventProps.Segment);
            }

            if (analytics.Tradefact)
            {
                ClaimsIdentity identity = (ClaimsIdentity)user.Identity;
                string userId = identity.FindFirst(q => q.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
                await CreateNewTradefactActivity(new Guid(userId), org.Id, eventProps.Tradefact, user);
            }
        }

        public async Task TrackEvent(ClaimsPrincipal user, Guid orgId, string eventName, TrackWith analytics, EventProps eventProps)
        {
            if (analytics.Segment)
            {
                await _analyticsService.TrackEvent(user, orgId, eventName, eventProps.Segment);
            }

            if (analytics.Tradefact)
            {
                ClaimsIdentity identity = (ClaimsIdentity)user.Identity;
                string userId = identity.FindFirst(q => q.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
                await CreateNewTradefactActivity(new Guid(userId), orgId, eventProps.Tradefact, user);
            }
        }

        public async Task TrackEvent(string userId, string userFullName, string userEmail, Organisation org, Organisation invitingOrg, string eventName, TrackWith analytics, EventProps eventProps)
        {
            if (analytics.Segment)
            {
                await _analyticsService.TrackEvent(userId, userFullName, userEmail, org, invitingOrg.Name, eventName, eventProps.Segment);
            }

            if (analytics.Tradefact)
            {
                await CreateNewTradefactActivity(new Guid(userId), invitingOrg.Id, eventProps.Tradefact);
            }
        }

        public async Task TrackAnonymousEvent(Guid anonymousId, string name, string email, string companyName, string eventName, TrackWith analytics, EventProps eventProps)
        {
            if (analytics.Segment)
            {
                await _analyticsService.TrackAnonymousEvent(anonymousId, name, email, companyName, eventName, eventProps.Segment);
            }
        }

        private async Task CreateNewTradefactActivity(Guid userId, Guid organisationId, TradefactEventProps eventProps, ClaimsPrincipal user = null)
        {
            (string text1, string text2, string description, string serialisedData) = await GetActivityData(eventProps, user);

            var activity = new Activity
            {
                UserId = userId,
                OrganisationId = organisationId,
                Type = eventProps.Type,
                Text1 = text1,
                Text2 = text2,
                Description = description,
                Entity = eventProps.Entity,
                Data = serialisedData
            };

            _context.Add(activity);

            _ = await _context.SaveChangesAsync();
        }

        private async Task<PurchaseOrderActivityResource> GetPurchaseOrder(Guid id)
        {
            PurchaseOrderActivityResource purchaseOrder = await _context.PurchaseOrders.Select(s => new PurchaseOrderActivityResource
            {
                Id = s.Id,
                SupplierId = s.SupplierId,
                PurchaseOrderNumber = s.PurchaseOrderNumber,
                Status = s.Status
            }).SingleOrDefaultAsync(q => q.Id == id);
            return purchaseOrder;
        }

        private async Task<(string text1, string text2, string description, string serialisedData)> GetPOStatus(string id)
        {
            PurchaseOrderActivityResource purchaseOrder = await GetPurchaseOrder(new Guid(id));

            if (purchaseOrder == null)
            {
                return ("", "", "", "");
            }

            var serialisedData = JsonConvert.SerializeObject(purchaseOrder);

            return (purchaseOrder.PurchaseOrderNumber, "", $"Status changed to {purchaseOrder.Status}", serialisedData);
        }

        private async Task<(string text1, string text2, string description, string serialisedData)> GetPOComment(string id, string customDescription)
        {
            PurchaseOrderActivityResource purchaseOrder = await GetPurchaseOrder(new Guid(id));

            if (purchaseOrder == null)
            {
                return ("", "", "", "");
            }

            var serialisedData = JsonConvert.SerializeObject(purchaseOrder);

            return (purchaseOrder.PurchaseOrderNumber, "", $"New Comment: {customDescription}", serialisedData);
        }

        private async Task<Shipment> GetShipment(Guid id)
        {
            Shipment shipment = await _context.Shipments.FindAsync(id);
            return shipment;
        }

        private async Task<(string text1, string text2, string description, string serialisedData)> GetFMStatus(string id, string customDescription)
        {
            var shipment = await GetShipment(new Guid(id));

            if (shipment == null)
            {
                return ("", "", "", "");
            }

            var freightMovement = await _context.FreightMovements.FindAsync(shipment.FreightMovementId);

            if (freightMovement == null)
            {
                return ("", "", "", "");
            }

            var freightMovementObjectToSerialise = freightMovement.Adapt<FreightMovementResource>();
            var serialisedData = JsonConvert.SerializeObject(freightMovementObjectToSerialise);

            return (freightMovement.Name, "", $"Status changed to {customDescription}", serialisedData);
        }

        private async Task<(string text1, string text2, string description, string serialisedData)> GetFMComment(string id, string customDescription)
        {
            var shipment = await GetShipment(new Guid(id));

            if (shipment == null)
            {
                return ("", "", "", "");
            }

            var freightMovement = await _context.FreightMovements.FindAsync(shipment.FreightMovementId);

            if (freightMovement == null)
            {
                return ("", "", "", "");
            }

            var freightMovementObjectToSerialise = freightMovement.Adapt<FreightMovementResource>();
            var serialisedData = JsonConvert.SerializeObject(freightMovementObjectToSerialise);

            return (freightMovement.Name, "", $"New Comment: {customDescription}", serialisedData);
        }

        private async Task<Product> GetProduct(Guid id)
        {
            Product product = await _context.Products.FindAsync(id);
            return product;
        }

        private async Task<(string text1, string text2, string description, string serialisedData)> GetProductInfo(string id, ClaimsPrincipal user)
        {
            var product = await GetProduct(new Guid(id));

            if (product == null)
            {
                return ("", "", "", "");
            }

            var productObjectToSerialise = product.Adapt<ProductResource>();
            var serialisedData = JsonConvert.SerializeObject(productObjectToSerialise);

            string userFullName = "";

            if (user != null)
            {
                ClaimsIdentity identity = (ClaimsIdentity)user.Identity;
                userFullName = identity.Claims.Where(c => c.Type == "name").Select(c => c.Value).SingleOrDefault();
            }

            return (product.Name, userFullName, "Product Info Updated", serialisedData);
        }

        private async Task<(string text1, string text2, string description, string serialisedData)> GetProductUpload(string id, string customDescription, ClaimsPrincipal user)
        {
            var product = await GetProduct(new Guid(id));

            if (product == null)
            {
                return ("", "", "", "");
            }

            var productObjectToSerialise = product.Adapt<ProductResource>();
            var serialisedData = JsonConvert.SerializeObject(productObjectToSerialise);

            string userFullName = "";

            if (user != null)
            {
                ClaimsIdentity identity = (ClaimsIdentity)user.Identity;
                userFullName = identity.Claims.Where(c => c.Type == "name").Select(c => c.Value).SingleOrDefault();
            }

            return (product.Name, userFullName, $"Product {customDescription} Uploaded", serialisedData);
        }

        private async Task<(string text1, string text2, string description, string serialisedData)> GetNetworkInvite(string emailAddress, string customDescription, ClaimsPrincipal user)
        {
            var original_invitation = await _context.InvitationLog.FirstOrDefaultAsync(q => q.EmailAddress == emailAddress);

            if (original_invitation == null)
            {
                return ("", "", "", "");
            }

            var serialisedData = JsonConvert.SerializeObject(original_invitation);

            string orgName = "";

            if (user != null)
            {
                ClaimsIdentity identity = (ClaimsIdentity)user.Identity;
                var orgId = identity.Claims.Where(c => c.Type == "OrganisationId")
                   .Select(c => c.Value).SingleOrDefault();
                var org = await _context.Organisations.FirstOrDefaultAsync(x => x.Id == new Guid(orgId));
                orgName = org.Name;
            }

            return (original_invitation.CompanyName, orgName, customDescription, serialisedData);
        }

        private async Task<(string text1, string text2, string description, string serialisedData)> GetActivityData(TradefactEventProps eventProps, ClaimsPrincipal user)
        {
            ActivityTypeEnum type = eventProps.Type;
            ActivityEntityTypeEnum entity = eventProps.Entity;

            if (type == ActivityTypeEnum.STATUS && entity == ActivityEntityTypeEnum.PURCHASEORDER) {
                return _ = await GetPOStatus(eventProps.Reference);
            }

            if (type == ActivityTypeEnum.STATUS && entity == ActivityEntityTypeEnum.SHIPMENT)
            {
                return _ = await GetFMStatus(eventProps.Reference, eventProps.CustomDescription);
            }

            if (type == ActivityTypeEnum.COMMENTS && entity == ActivityEntityTypeEnum.PURCHASEORDER)
            {
                return _ = await GetPOComment(eventProps.Reference, eventProps.CustomDescription);
            }

            if (type == ActivityTypeEnum.COMMENTS && entity == ActivityEntityTypeEnum.SHIPMENT)
            {
                return _ = await GetFMComment(eventProps.Reference, eventProps.CustomDescription);
            }

            if (type == ActivityTypeEnum.INFO && entity == ActivityEntityTypeEnum.PRODUCT)
            {
                return _ = await GetProductInfo(eventProps.Reference, user);
            }

            if (type == ActivityTypeEnum.UPLOAD && entity == ActivityEntityTypeEnum.PRODUCT)
            {
                return _ = await GetProductUpload(eventProps.Reference, eventProps.CustomDescription, user);
            }

            if (type == ActivityTypeEnum.INVITE && entity == ActivityEntityTypeEnum.NETWORK)
            {
                return _ = await GetNetworkInvite(eventProps.Reference, eventProps.CustomDescription, user);
            }

            return ("", "", "", "");
        }
    }
}
