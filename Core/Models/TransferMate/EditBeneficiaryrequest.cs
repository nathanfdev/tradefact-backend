using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Models.TransferMate
{
    /// <summary>
    ///
    /// </summary>
    [DataContract]
  public class EditBeneficiaryrequest
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
          sb.Append("class EditBeneficiaryrequest {\n");
          sb.Append("  Id: ").Append(Id).Append("\n");
          sb.Append("  PayeeName: ").Append(PayeeName).Append("\n");
          sb.Append("  AccountType: ").Append(AccountType).Append("\n");
          sb.Append("  PayeeAddress: ").Append(PayeeAddress).Append("\n");
          sb.Append("  PayeeAddressToAccountType: ").Append(PayeeAddressToAccountType).Append("\n");
          sb.Append("  BankBranchAddressToAccountType: ").Append(BankBranchAddressToAccountType).Append("\n");
          sb.Append("  ForFurtherCreditTo: ").Append(ForFurtherCreditTo).Append("\n");
          sb.Append("  DefPaymentReference: ").Append(DefPaymentReference).Append("\n");
          sb.Append("  Email: ").Append(Email).Append("\n");
          sb.Append("  EmailAlertFlag: ").Append(EmailAlertFlag).Append("\n");
          sb.Append("}\n");
          return sb.ToString();
      }

        /// <summary>
        /// Gets or Sets AccountType
        /// </summary>
        [DataMember(Name="account_type", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "account_type")]
    public string AccountType { get; set; }

        /// <summary>
        /// Gets or Sets BankBranchAddressToAccountType
        /// </summary>
        [DataMember(Name="bank_branch_address_to_account_type", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "bank_branch_address_to_account_type")]
    public string BankBranchAddressToAccountType { get; set; }

        /// <summary>
        /// Gets or Sets DefPaymentReference
        /// </summary>
        [DataMember(Name="def_payment_reference", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "def_payment_reference")]
    public string DefPaymentReference { get; set; }

        /// <summary>
        /// Gets or Sets Email
        /// </summary>
        [DataMember(Name="email", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "email")]
    public string Email { get; set; }

        /// <summary>
        /// Gets or Sets EmailAlertFlag
        /// </summary>
        [DataMember(Name="email_alert_flag", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "email_alert_flag")]
    public long? EmailAlertFlag { get; set; }

        /// <summary>
        /// Gets or Sets ForFurtherCreditTo
        /// </summary>
        [DataMember(Name="for_further_credit_to", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "for_further_credit_to")]
    public string ForFurtherCreditTo { get; set; }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name="id", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "id")]
    public int? Id { get; set; }

        /// <summary>
        /// Gets or Sets PayeeAddress
        /// </summary>
        [DataMember(Name="payee_address", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "payee_address")]
    public string PayeeAddress { get; set; }

        /// <summary>
        /// Gets or Sets PayeeAddressToAccountType
        /// </summary>
        [DataMember(Name="payee_address_to_account_type", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "payee_address_to_account_type")]
    public string PayeeAddressToAccountType { get; set; }

        /// <summary>
        /// Gets or Sets PayeeName
        /// </summary>
        [DataMember(Name="payee_name", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "payee_name")]
    public string PayeeName { get; set; }
  }
}
