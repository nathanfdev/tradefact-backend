using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NSwag.Annotations;
using Tradefact.Api.Model.Collab;
using Tradefact.Application.Common.Interfaces;
using Tradefact.Application.Models;
using Tradefact.Application.Models.Activity;
using Tradefact.Application.Products.Queries;
using Tradefact.Application.PurchaseOrders.Queries;
using Tradefact.Data;

namespace Tradefact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : BaseApiController
    {

        protected readonly IMediator _mediator;
        private readonly ICurrentUserService _userService;
        private readonly ITradefactActivityService _tradefactActivityService;

        public ChatController(TradefactDbContext context, ICurrentUserService userService, IMediator mediator, ITradefactActivityService tradefactActivityService) : base(context)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _tradefactActivityService = tradefactActivityService ?? throw new ArgumentNullException(nameof(tradefactActivityService));
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(Room), Description = "Ok Result")]
        [Route("room/{roomid:guid}")]
        public async Task<IActionResult> GetRoomById(Guid roomid)
        {
            Room room = await _context.Rooms.SingleOrDefaultAsync(q => q.Id == roomid);
            if (room == null)
            {
                return new NotFoundResult();
            }
            room.Messages = await _mediator.Send(new GetRoomMessageListQuery
            {
                RoomId = room.Id
            });
            return new OkObjectResult(room);
        }

        [HttpPost]
        [SwaggerResponse("200", typeof(Room), Description = "Ok Result")]
        [Route("room/{roomid:guid}/message")]
        public async Task<IActionResult> PostMessage(Guid roomid, [FromBody] MessagePost post)
        {
            Message message = new Message { Id = Guid.NewGuid(), RoomId = roomid, Comment = this.WrapHyperlinks(post.Comment) };
            _context.Messages.Add(message);
            _ = await _context.SaveChangesAsync();

            Room room = await _context.Rooms.Include(i => i.Messages).SingleOrDefaultAsync(q => q.Id == roomid);

            if (room == null)
            {
                return new NotFoundResult();
            }

            (string text1, string text2, string serialisedData) = await GetCommentsData(roomid, post.Entity);

            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            string userId = identity.FindFirst(q => q.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            var activity = new Activity
            {
                UserId = new Guid(userId),
                OrganisationId = this.OrganisationId,
                Type = ActivityTypeEnum.COMMENTS,
                Text1 = text1,
                Text2 = text2,
                Description = $"New Comment: {post.Comment}",
                Entity = ActivityEntityTypeEnum.PURCHASEORDER,
                Data = serialisedData
            };

            await _tradefactActivityService.TrackEvent(
                User,
                this.OrganisationId,
                "new_comment_added",
                new TrackWith { Segment = false, Tradefact = true },
                new EventProps { Tradefact = new TradefactEventProps { Reference = roomid.ToString(), Type = ActivityTypeEnum.COMMENTS, Entity = post.Entity, CustomDescription = post.Comment } }
            );

            return Ok(room);
        }


        private string WrapHyperlinks(string comment)
        {
            Regex regx = new Regex(@"\b(http|ftp|https)?(://)?([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?");
            return regx.Replace(comment, new MatchEvaluator(ReplaceURl));
        }

        static string ReplaceURl(Match m)
        {
            string x = m.ToString();
            x = "<a href=\"" + x + "\">" + x + "</a>";
            return x;
        }

        [HttpPut]
        [SwaggerResponse("200", typeof(Room), Description = "Created result")]
        [Route("room/{id:guid}/message")]
        public virtual async Task<IActionResult> Update(Guid id, [FromBody] MessagePost messagePost)
        {
            Message existing_message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
            if (existing_message != null)
            {
                existing_message.Comment = messagePost.Comment;
                await _context.SaveChangesAsync();
                return new OkObjectResult(existing_message);
            }
            return new BadRequestResult();
        }

        [HttpDelete]
        [SwaggerResponse("200", typeof(Room), Description = "Deleted result")]
        [Route("room/{id:guid}")]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            var message = await _context.Messages.SingleOrDefaultAsync(s => s.Id == id);
            if (message != null)
            {
                _context.Messages.Remove(message);
                _ = await _context.SaveChangesAsync();
                return new OkObjectResult(new { });
            }
            return new BadRequestResult();
        }       

        private async Task<(string text1, string text2, string serialisedData)> GetCommentsData(Guid id, ActivityEntityTypeEnum entity)
        {
            if (entity == ActivityEntityTypeEnum.PURCHASEORDER)
            {
                PurchaseOrderActivityResource purchaseOrder = await _context.PurchaseOrders.Select(s => new PurchaseOrderActivityResource
                {
                    Id = s.Id,
                    SupplierId = s.SupplierId,
                    PurchaseOrderNumber = s.PurchaseOrderNumber,
                    Status = s.Status
                }).SingleOrDefaultAsync(q => q.Id == id);
                var serialisedData = JsonConvert.SerializeObject(purchaseOrder);
                return (purchaseOrder.PurchaseOrderNumber, "", serialisedData);
            }

            if (entity == ActivityEntityTypeEnum.SHIPMENT)
            {
                var shipment = await _context.Shipments.FindAsync(id);
                var freightMovement = await _context.FreightMovements.FindAsync(shipment.FreightMovementId);
                var freightMovementObjectToSerialise = freightMovement.Adapt<FreightMovementResource>();
                var serialisedData = JsonConvert.SerializeObject(freightMovementObjectToSerialise);
                return (freightMovement.Name, "", serialisedData);
            }

            return ("", "", "");
        }
    }
}
