using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Models.TransferMate
{
    /// <summary>
    ///
    /// </summary>
    [DataContract]
  public class SendQueryrequest
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
          sb.Append("class SendQueryrequest {\n");
          sb.Append("  RefNumber: ").Append(RefNumber).Append("\n");
          sb.Append("  QuoteQuestion: ").Append(QuoteQuestion).Append("\n");
          sb.Append("}\n");
          return sb.ToString();
      }

        /// <summary>
        /// Gets or Sets QuoteQuestion
        /// </summary>
        [DataMember(Name="quote_question", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "quote_question")]
    public string QuoteQuestion { get; set; }

        /// <summary>
        /// Gets or Sets RefNumber
        /// </summary>
        [DataMember(Name="ref_number", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "ref_number")]
    public string RefNumber { get; set; }
  }
}
