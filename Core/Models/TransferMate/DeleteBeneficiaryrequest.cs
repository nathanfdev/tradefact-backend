using Newtonsoft.Json;

namespace Core.Models.TransferMate
{
    public class DeleteBeneficiaryRequest
    {
        [JsonProperty("id")]
        public long Id { get; set; }
    }
}
