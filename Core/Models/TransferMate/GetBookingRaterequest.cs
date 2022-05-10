using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Models.TransferMate
{
    /// <summary>
    ///
    /// </summary>
    [DataContract]
  public class GetBookingRaterequest
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
          sb.Append("class GetBookingRaterequest {\n");
          sb.Append("  BankIdFrom: ").Append(BankIdFrom).Append("\n");
          sb.Append("  BankPayType: ").Append(BankPayType).Append("\n");
          sb.Append("  BeneficiaryData: ").Append(BeneficiaryData).Append("\n");
          sb.Append("}\n");
          return sb.ToString();
      }

        /// <summary>
        /// Gets or Sets BankIdFrom
        /// </summary>
        [DataMember(Name="bank_id_from", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "bank_id_from")]
    public long? BankIdFrom { get; set; }

        /// <summary>
        /// Gets or Sets BankPayType
        /// </summary>
        [DataMember(Name="bank_pay_type", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "bank_pay_type")]
    public long? BankPayType { get; set; }

        /// <summary>
        /// Gets or Sets BeneficiaryData
        /// </summary>
        [DataMember(Name="beneficiary_data", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "beneficiary_data")]
    public List<BookingBeneficiaryExample1> BeneficiaryData { get; set; }
  }
}
