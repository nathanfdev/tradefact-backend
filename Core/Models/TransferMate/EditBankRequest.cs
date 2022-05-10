using Newtonsoft.Json;

namespace Core.Models.TransferMate
{
    public class EditBankRequest
    {
        [JsonProperty("account_name")]
        public string AccountName { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("is_active")]
        public long IsActive { get; set; }
    }
}