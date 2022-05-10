using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Models.TransferMate
{
    /// <summary>
    ///
    /// </summary>
    [DataContract]
  public class BookingBeneficiaryExample2
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
          sb.Append("class BookingBeneficiaryExample2 {\n");
          sb.Append("  BeneficiaryDataId: ").Append(BeneficiaryDataId).Append("\n");
          sb.Append("  BeneficiaryDataCurrency: ").Append(BeneficiaryDataCurrency).Append("\n");
          sb.Append("  BeneficiaryDataAmount: ").Append(BeneficiaryDataAmount).Append("\n");
          sb.Append("  BeneficiaryDataUserReference: ").Append(BeneficiaryDataUserReference).Append("\n");
          sb.Append("  BeneficiaryDataTransferReason: ").Append(BeneficiaryDataTransferReason).Append("\n");
          sb.Append("  ReconciliationId: ").Append(ReconciliationId).Append("\n");
          sb.Append("}\n");
          return sb.ToString();
      }

        /// <summary>
        /// Gets or Sets BeneficiaryDataAmount
        /// </summary>
        [DataMember(Name="beneficiary_data_amount", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "beneficiary_data_amount")]
    public decimal? BeneficiaryDataAmount { get; set; }

        /// <summary>
        /// Gets or Sets BeneficiaryDataCurrency
        /// </summary>
        [DataMember(Name="beneficiary_data_currency", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "beneficiary_data_currency")]
    public string BeneficiaryDataCurrency { get; set; }

        /// <summary>
        /// Gets or Sets BeneficiaryDataId
        /// </summary>
        [DataMember(Name="beneficiary_data_id", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "beneficiary_data_id")]
    public long? BeneficiaryDataId { get; set; }

        /// <summary>
        /// Gets or Sets BeneficiaryDataTransferReason
        /// </summary>
        [DataMember(Name="beneficiary_data_transfer_reason", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "beneficiary_data_transfer_reason")]
    public string BeneficiaryDataTransferReason { get; set; }

        /// <summary>
        /// Gets or Sets BeneficiaryDataUserReference
        /// </summary>
        [DataMember(Name="beneficiary_data_user_reference", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "beneficiary_data_user_reference")]
    public string BeneficiaryDataUserReference { get; set; }

        /// <summary>
        /// Gets or Sets ReconciliationId
        /// </summary>
        [DataMember(Name="reconciliation_id", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "reconciliation_id")]
    public string ReconciliationId { get; set; }
  }
}
