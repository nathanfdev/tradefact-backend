using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Models.TransferMate
{
    /// <summary>
    ///
    /// </summary>
    [DataContract]
  public class ConnectionHistoryrequest
  {
      /// <summary>
      /// Get the JSON string presentation of the object
      /// </summary>
    /// <returns>JSON string presentation of the object</returns>
      public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented) ;

      /// <summary>
      /// Get the string presentation of the object
      /// </summary>
    /// <returns>String presentation of the object</returns>
      public override string ToString()
      {
          var sb = new StringBuilder();
          sb.Append("class ConnectionHistoryrequest {\n");
          sb.Append("  DateFrom: ").Append(DateFrom).Append("\n");
          sb.Append("  DateTo: ").Append(DateTo).Append("\n");
          sb.Append("  OrderBy: ").Append(OrderBy).Append("\n");
          sb.Append("  OrderType: ").Append(OrderType).Append("\n");
          sb.Append("  PageNo: ").Append(PageNo).Append("\n");
          sb.Append("}\n");
          return sb.ToString();
      }

        /// <summary>
        /// Gets or Sets DateFrom
        /// </summary>
        [DataMember(Name="date_from", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "date_from")]
    public string DateFrom { get; set; }

        /// <summary>
        /// Gets or Sets DateTo
        /// </summary>
        [DataMember(Name="date_to", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "date_to")]
    public string DateTo { get; set; }

        /// <summary>
        /// Gets or Sets OrderBy
        /// </summary>
        [DataMember(Name="order_by", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "order_by")]
    public string OrderBy { get; set; }

        /// <summary>
        /// Gets or Sets OrderType
        /// </summary>
        [DataMember(Name="order_type", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "order_type")]
    public string OrderType { get; set; }

        /// <summary>
        /// Gets or Sets PageNo
        /// </summary>
        [DataMember(Name="page_no", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "page_no")]
    public string PageNo { get; set; }
  }
}
