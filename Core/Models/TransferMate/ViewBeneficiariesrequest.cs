using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Models.TransferMate
{
    /// <summary>
    ///
    /// </summary>
    [DataContract]
  public class ViewBeneficiariesrequest
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
          sb.Append("class ViewBeneficiariesrequest {\n");
          sb.Append("  BeneficiaryName: ").Append(BeneficiaryName).Append("\n");
          sb.Append("  BeneficiaryCountry: ").Append(BeneficiaryCountry).Append("\n");
          sb.Append("}\n");
          return sb.ToString();
      }

        /// <summary>
        /// Gets or Sets BeneficiaryCountry
        /// </summary>
        [DataMember(Name="beneficiary_country", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "beneficiary_country")]
    public string BeneficiaryCountry { get; set; }

        /// <summary>
        /// Gets or Sets BeneficiaryName
        /// </summary>
        [DataMember(Name="beneficiary_name", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "beneficiary_name")]
    public string BeneficiaryName { get; set; }
  }
}
