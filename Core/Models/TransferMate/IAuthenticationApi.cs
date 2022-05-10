using System.Threading.Tasks;
using Refit;

namespace Core.Models.TransferMate
{
    public interface IAuthenticationApi
    {
        [Post("/connect")]
        Task<ConnectResponse> Connect([Body] ConnectRequest connectRequest);

        [Get("/connection_history")]
        [Headers("Authorization: Bearer")]
        Task<ConnectionHistoryResponse> ConnectionHistory([Body] ConnectionHistoryRequest connectHistoryRequest);

        [Get("/disconnect")]
        Task Disconnect();
    }
}