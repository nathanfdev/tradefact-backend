using Newtonsoft.Json;

namespace Core.Models.TransferMate
{
    public class ViewBanksRequest
    {
        [JsonProperty("page_no")]
        public long PageNo { get; set; }

        [JsonProperty("show")]
        public long Show { get; set; }

        [JsonProperty("with_pagination_data")]
        public long WithPaginationData { get; set; }
    }
}