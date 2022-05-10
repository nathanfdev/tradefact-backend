using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Models.TransferMate
{
    [DataContract]
  public class GetRatesrequest
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
          sb.Append("class GetRatesrequest {\n");
          sb.Append("  Amount: ").Append(Amount).Append("\n");
          sb.Append("  Pairs: ").Append(Pairs).Append("\n");
          sb.Append("}\n");
          return sb.ToString();
      }

        /// <summary>
        /// Gets or Sets Amount
        /// </summary>
        [DataMember(Name="amount", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "amount")]
    public long? Amount { get; set; }

        /// <summary>
        /// Gets or Sets Pairs
        /// </summary>
        [DataMember(Name="pairs", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "pairs")]
    public List<Pair> Pairs { get; set; } = new List<Pair>();
  }
}
