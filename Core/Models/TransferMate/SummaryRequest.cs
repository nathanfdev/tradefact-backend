using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Models.TransferMate
{
    /// <summary>
    ///
    /// </summary>
    [DataContract]
  public class SummaryRequest
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
          sb.Append("class SummaryRequest {\n");
          sb.Append("  RefNumber: ").Append(RefNumber).Append("\n");
          sb.Append("  DateFrom: ").Append(DateFrom).Append("\n");
          sb.Append("  DateTo: ").Append(DateTo).Append("\n");
          sb.Append("  CurrencyFrom: ").Append(CurrencyFrom).Append("\n");
          sb.Append("  Username: ").Append(Username).Append("\n");
          sb.Append("  Beneficiary: ").Append(Beneficiary).Append("\n");
          sb.Append("  BankIdFrom: ").Append(BankIdFrom).Append("\n");
          sb.Append("  TransactionsStatuses: ").Append(TransactionsStatuses).Append("\n");
          sb.Append("}\n");
          return sb.ToString();
      }

        /// <summary>
        /// Gets or Sets BankIdFrom
        /// </summary>
        [DataMember(Name="bank_id_from", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "bank_id_from")]
    public string BankIdFrom { get; set; }

        /// <summary>
        /// Gets or Sets Beneficiary
        /// </summary>
        [DataMember(Name="beneficiary", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "beneficiary")]
    public string Beneficiary { get; set; }

        /// <summary>
        /// Gets or Sets CurrencyFrom
        /// </summary>
        [DataMember(Name="currency_from", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "currency_from")]
    public string CurrencyFrom { get; set; }

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
        /// Gets or Sets RefNumber
        /// </summary>
        [DataMember(Name="ref_number", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "ref_number")]
    public string RefNumber { get; set; }

        /// <summary>
        /// Gets or Sets TransactionsStatuses
        /// </summary>
        [DataMember(Name="transactions_statuses", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "transactions_statuses")]
    public string TransactionsStatuses { get; set; }

        /// <summary>
        /// Gets or Sets Username
        /// </summary>
        [DataMember(Name="username", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "username")]
    public string Username { get; set; }
  }
}
