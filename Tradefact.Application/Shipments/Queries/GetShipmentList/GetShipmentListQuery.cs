using Core.Common;
using Core.Enums;
using Core.Models;
using Core.Models.Criteria;
using Dapper;
using Mapster;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.Application.Common.Interfaces;
using Tradefact.Application.Helpers;
using Tradefact.Application.Models;
using Tradefact.Application.Models.Shipment;
using Tradefact.Data;
using X.PagedList;



namespace Tradefact.Application.Shipments.Queries.GetShipmentList
{
    public class GetShipmentListQuery : IRequest<IPagedList<ShipmentInfo>>
    {
        public OrganisationTypeEnum OrganisationType { get; set; }
        public Guid OrganisationId { get; set; }
        public ShipmentSearchCriteria Criteria { get; set; }

        public PagedResultParameters Paging { get; set; }

        public class GetShipmentListQueryHandler : IRequestHandler<GetShipmentListQuery, IPagedList<ShipmentInfo>>
        {
            private readonly IDbConnection _connection;
            private readonly ICurrentUserService _userService;

            public GetShipmentListQueryHandler(IDbConnection connection, ICurrentUserService userService)
            {
                _connection = connection;
                _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            }

            public async Task<IPagedList<ShipmentInfo>> Handle(GetShipmentListQuery request, System.Threading.CancellationToken cancellationToken)
            {
                var sQuery = new ShipmentQueries().GetBuildQuery(request.Criteria, request.OrganisationType, _userService.Email);

                using (IDbConnection conn = _connection)
                {
                    conn.Open();

                    using (var multi = await conn.QueryMultipleAsync(sQuery,
                    new
                    {
                        request.OrganisationId,
                        @ShipmentMethod = request.Criteria.ShipmentMethod.GetValueOrDefault(),
                        @search = BuildSearchParam(request.Criteria.Search),
                        @PageSize = request.Criteria.HasException ? int.MaxValue : request.Paging.PageSize,
                        @PageNumber = request.Criteria.HasException ? 1 : request.Paging.PageNumber
                    }, commandTimeout: 60))
                    {
                        List<ShipmentInfo> query_result_shipments = (await multi.ReadAsync<ShipmentInfo>()).ToList();

                        List<ShipmentScheduleInfo> query_result_shipment_schedules = (await multi.ReadAsync<ShipmentScheduleInfo>()).ToList();

                        List<ShipmentPurchaseOrderInfo> query_po_numbers = (await multi.ReadAsync<ShipmentPurchaseOrderInfo>()).ToList();

                        int ShipmentsCount = await multi.ReadFirstAsync<int>();

                        foreach (ShipmentInfo shipment in query_result_shipments)
                        {
                            shipment.PurchaseOrderInfo = query_po_numbers
                                .Where(q => q.ShipmentId == shipment.Id)
                                .ToList();
                        }

                        if (request.Criteria.HasException)
                        {
                            IPagedList<ShipmentInfo> all_shipments_with_exceptions = await query_result_shipments.Where(q => q.HasExceptions).ToPagedListAsync(request.Paging.PageNumber, request.Paging.PageSize);
                            foreach (ShipmentInfo ex_shipment in all_shipments_with_exceptions)
                            {
                                ex_shipment.Schedules = query_result_shipment_schedules.OrderBy(o => o.GoodsReady).Where(q => q.ShipmentId == ex_shipment.Id).ToList();
                                if (ex_shipment.Schedules?.Count > 0)
                                {
                                    ex_shipment.GoodsReadyMin = ex_shipment.Schedules.Min(s => s.GoodsReady);
                                    ex_shipment.GoodsReadyMax = ex_shipment.Schedules.Max(s => s.GoodsReady);

                                    ex_shipment.GoodsReadyDates = ex_shipment.Schedules.OrderBy(o => o.GoodsReady).Select(s => s.GoodsReady).Distinct().ToArray();
                                    ex_shipment.NoOfGoodsReadyDates = ex_shipment.GoodsReadyDates.Count();
                                }
                                else
                                {
                                    ex_shipment.GoodsReadyMin = ex_shipment.GoodsReady;
                                    ex_shipment.GoodsReadyMax = ex_shipment.GoodsReady;

                                    ex_shipment.GoodsReadyDates = new DateTime[] { ex_shipment.GoodsReady };
                                    ex_shipment.NoOfGoodsReadyDates = ex_shipment.GoodsReadyDates.Count();
                                }
                            }
                            return all_shipments_with_exceptions;
                        }

                        foreach (ShipmentInfo shipment in query_result_shipments)
                        {
                            shipment.Schedules = query_result_shipment_schedules.OrderBy(o => o.GoodsReady).Where(q => q.ShipmentId == shipment.Id).ToList();
                            if (shipment.Schedules?.Count > 0)
                            {
                                shipment.GoodsReadyMin = shipment.Schedules.Min(s => s.GoodsReady);
                                shipment.GoodsReadyMax = shipment.Schedules.Max(s => s.GoodsReady);

                                shipment.GoodsReadyDates = shipment.Schedules.OrderBy(o => o.GoodsReady).Select(s => s.GoodsReady).Distinct().ToArray();
                                shipment.NoOfGoodsReadyDates = shipment.GoodsReadyDates.Count();
                            }
                            else
                            {
                                shipment.GoodsReadyMin = shipment.GoodsReady;
                                shipment.GoodsReadyMax = shipment.GoodsReady;
                                shipment.GoodsReadyDates = new DateTime[] { shipment.GoodsReady };
                                shipment.NoOfGoodsReadyDates = shipment.GoodsReadyDates.Count();
                            }
                        }

                        return new StaticPagedList<ShipmentInfo>(query_result_shipments.Adapt<List<ShipmentInfo>>(), request.Paging.PageNumber, request.Paging.PageSize, ShipmentsCount);
                    }
                }
            }


            private string BuildSearchParam(string value)
            {
                return String.IsNullOrEmpty(value) ? null : $"%{value}%";
            }
        }
    }
}
