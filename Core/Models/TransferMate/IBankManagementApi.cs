using Refit;
using System.Threading.Tasks;

namespace Core.Models.TransferMate
{
    public interface IBankManagementApi
    {
        [Put("/add_bank")]
        [Headers("Authorization: Bearer")]
        Task<TransferMate.AddBankResponse> AddBank([Body] AddBankRequest addBankRequest);

        [Post("/edit_bank")]
        [Headers("Authorization: Bearer")]
        Task<TransferMate.EditBankResponse> EditBank([Body] TransferMate.EditBankRequest editBankRequest);

        [Post("/verify_bank")]
        [Headers("Authorization: Bearer")]
        Task<TransferMate.VerifyBankResponse> VerifyBank([Body] TransferMate.VerifyBankRequest verifyBankRequest);

        [Post("/view_banks")]
        [Headers("Authorization: Bearer")]
        Task<ViewBanksResponse> ViewBanks([Body] ViewBanksRequest viewBanksRequest);
    }
}
