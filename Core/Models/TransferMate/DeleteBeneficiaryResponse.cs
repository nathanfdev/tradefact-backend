using Newtonsoft.Json;

namespace Core.Models.TransferMate
{
    public class DeleteBeneficiaryResponse
    {
        [JsonProperty("stat")]
        public string Stat { get; set; }
    }
}