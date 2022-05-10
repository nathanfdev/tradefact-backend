using Dapper;
using Mapster;
using MediatR;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Common.Interfaces;
using Core.Common;
using X.PagedList;

namespace Tradefact.Application.PurchaseOrders.Queries
{
    public class GetPurchaseOrderTimelineQuery : IRequest<IPagedList<PurchaseOrderEventResource>>
    {
        public Guid PurchaseOrderId { get; set; }
        public PagedResultParameters Paging { get; set; }

        public class GetPurchaseOrderTimelineQueryHandler : IRequestHandler<GetPurchaseOrderTimelineQuery, IPagedList<PurchaseOrderEventResource>>
        {
            private readonly IDbConnection _connection;

            public GetPurchaseOrderTimelineQueryHandler(IDbConnection connection)
            {
                _connection = connection;
            }

            public async Task<IPagedList<PurchaseOrderEventResource>> Handle(GetPurchaseOrderTimelineQuery request, System.Threading.CancellationToken cancellationToken)
            {

                string sQuery = @$"SELECT EV.[PurchaseOrderId], EV.[ActionDate] [EventTime], EV.[Description],  P.FullName, P.ProfileImage
                                        FROM [dbo].[PurchaseOrderEvents] EV
                                        LEFT JOIN [dbo].[vwUserProfiles] P ON P.Id = EV.[ActionedBy] 
                                    WHERE EV.[PurchaseOrderId] = @PurchaseOrderId
                                    ORDER BY EV.[ActionDate] desc;";

                using (IDbConnection conn = _connection)
                {
                    conn.Open();
                    IEnumerable<PurchaseOrderEventResource> query_result = await conn.QueryAsync<PurchaseOrderEventResource>(sQuery, new
                    {
                        @PurchaseOrderId = request.PurchaseOrderId
                    });
                    IPagedList<PurchaseOrderEventResource> purchase_order_timeline = await query_result.Adapt<List<PurchaseOrderEventResource>>().ToPagedListAsync(request.Paging.PageNumber, request.Paging.PageSize);
                    return purchase_order_timeline;
                }

            }
        }
    }
}
