using Core.Common;
using Core.Models;
using Core.Models.Extended;
using Dapper;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Tradefact.Application.Models.Collab;
using X.PagedList;

namespace Tradefact.Application.Products.Queries
{
    public class GetRoomMessageListQuery : IRequest<List<Message>>
    {
        public Guid? RoomId { get; set; }

        public class GetRoomMessageListQueryHandler : IRequestHandler<GetRoomMessageListQuery, List<Message>>
        {
            private readonly IDbConnection _connection;

            public GetRoomMessageListQueryHandler(IDbConnection connection)
            {
                _connection = connection;
            }

            public async Task<List<Message>> Handle(GetRoomMessageListQuery request, System.Threading.CancellationToken cancellationToken)
            {

                string sQuery = @$"SELECT M.*, P.FullName, P.ProfileImage
                                     FROM [collab].[Messages] M
                                        INNER JOIN [dbo].[vwUserProfiles] P ON P.Email = M.CreatedByUser 
                                    WHERE M.RoomId = @RoomId
                                    ORDER BY M.CreationDateInternal";

                using (IDbConnection conn = _connection)
                {
                    conn.Open();

                    IEnumerable<ExtendedMessage> query_result = await conn.QueryAsync<ExtendedMessage>(sQuery, new
                    {
                        request.RoomId,
                    });
                    return query_result.Adapt<List<Message>>();
                }
            }

        }

    }
}
