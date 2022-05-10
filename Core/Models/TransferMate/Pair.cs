using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Models.TransferMate
{
    /// <summary>
    ///
    /// </summary>
    [DataContract]
  public class Pair
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
          sb.Append("class Pair {\n");
          sb.Append("  Src: ").Append(Src).Append("\n");
          sb.Append("  Dst: ").Append(Dst).Append("\n");
          sb.Append("}\n");
          return sb.ToString();
      }

        /// <summary>
        /// Gets or Sets Dst
        /// </summary>
        [DataMember(Name="dst", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "dst")]
    public string Dst { get; set; }

        /// <summary>
        /// Gets or Sets Src
        /// </summary>
        [DataMember(Name="src", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "src")]
    public string Src { get; set; }
  }
}
