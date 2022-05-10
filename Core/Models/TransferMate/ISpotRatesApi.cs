using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Models.TransferMate
{
    public interface ISpotRatesApi
    {
        [Post("/get_rate")]
        [Headers("Authorization: Bearer")]
        Task<List<GetRatesResponse>> GetSpotRates([Body] TransferMate.GetRatesrequest getRatesrequest);
    }
}