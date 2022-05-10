using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Models.TransferMate
{
    [DataContract]
  public class GetInforequest
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
          sb.Append("class GetInforequest {\n");
          sb.Append("  RefNumber: ").Append(RefNumber).Append("\n");
          sb.Append("}\n");
          return sb.ToString();
      }

        /// <summary>
        /// Gets or Sets RefNumber
        /// </summary>
        [DataMember(Name="ref_number", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "ref_number")]
    public string RefNumber { get; set; }
  }
}
