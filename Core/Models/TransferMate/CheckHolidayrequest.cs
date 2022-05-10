using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Models.TransferMate
{
    /// <summary>
    ///
    /// </summary>
    [DataContract]
  public class CheckHolidayrequest
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
          sb.Append("class CheckHolidayrequest {\n");
          sb.Append("  Country: ").Append(Country).Append("\n");
          sb.Append("  Date: ").Append(Date).Append("\n");
          sb.Append("}\n");
          return sb.ToString();
      }

        /// <summary>
        /// Gets or Sets Country
        /// </summary>
        [DataMember(Name="country", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "country")]
    public string Country { get; set; }

        /// <summary>
        /// Gets or Sets Date
        /// </summary>
        [DataMember(Name="date", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "date")]
    public string Date { get; set; }
  }
}
