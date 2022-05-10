using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Models.TransferMate
{
    public interface IBeneficiaryAccountManagementApi
    {
        [Post("/add_beneficiary")]
        [Headers("Authorization: Bearer")]
        Task<List<TransferMate.AddBeneficiaryResponse>> AddBeneficiary([Body] TransferMate.AddBeneficiaryRequest addBeneficiaryRequest);

        [Post("/delete_beneficiary")]
        [Headers("Authorization: Bearer")]
        Task<List<TransferMate.DeleteBeneficiaryResponse>> DeleteBeneficiary([Body] TransferMate.DeleteBeneficiaryRequest deleteBeneficiaryRequest);
    }
}