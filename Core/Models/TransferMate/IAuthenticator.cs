using System.Threading.Tasks;

namespace Core.Models.TransferMate
{
    public interface IAuthenticator
    {
        Task<ConnectResponse> Authenticate();

        Task<string> GetToken();
    }
}
