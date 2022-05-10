using Core.Common;
using Core.Enums;
using Core.Extensions;
using Core.Interfaces;
using Core.Models;
using Core.Models.Criteria;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Api.Model;
using Tradefact.Api.Model.Shipment;
using Tradefact.Application.Models;
using Tradefact.Application.Products.Queries;
using Tradefact.Application.QuotationManagement.Queries.GetQuotationRequest;
using Tradefact.Application.Shipments.Commands;
using Tradefact.Application.Shipments.Queries.GetShipmentInfo;
using Tradefact.Application.Shipments.Queries.GetShipmentList;
using Tradefact.Data;
using X.PagedList;

namespace Tradefact.Api.Controllers
{
    public class NewsController : BaseApiController
    {

        private readonly ILogger<NewsController> _logger;
        protected readonly IMediator _mediator;

        public NewsController(TradefactDbContext context, IMediator mediator, ILogger<NewsController> logger) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        [SwaggerResponse("200", typeof(List<NewsSection>), Description = "OK Result")]
        [Route(nameof(List))]
        public async Task<IActionResult> List()
        {
            // Get the latest feed
            NewsFeed feed = await _context.NewsFeeds.Include(i => i.Sections).ThenInclude(i => i.Items).OrderByDescending(o => o.CreationDateInternal).FirstOrDefaultAsync(q => q.IsActive);
            return new OkObjectResult(feed.Sections.Select(s => new { WeekNo = feed.WeekNo, Title = s.Title, Reference = s.Reference, Items = s.Items.Select(i => new { Seq = i.SeqNo, NewsItem = i.Text }) }));
        }

    }
}
