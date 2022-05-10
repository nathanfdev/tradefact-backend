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
  public class AuthorizeRequest
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
          sb.Append("class AuthorizeRequest {\n");
          sb.Append("  LoginTermsOfUse: ").Append(LoginTermsOfUse).Append("\n");
          sb.Append("  Approve: ").Append(Approve).Append("\n");
          sb.Append("  AuthTransIds: ").Append(AuthTransIds).Append("\n");
          sb.Append("}\n");
          return sb.ToString();
      }

        /// <summary>
        /// Gets or Sets Approve
        /// </summary>
        [DataMember(Name="approve", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "approve")]
    public long? Approve { get; set; }

        /// <summary>
        /// Gets or Sets AuthTransIds
        /// </summary>
        [DataMember(Name="auth_trans_ids", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "auth_trans_ids")]
    public List<string> AuthTransIds { get; set; }

        /// <summary>
        /// Gets or Sets LoginTermsOfUse
        /// </summary>
        [DataMember(Name="login_terms_of_use", EmitDefaultValue=false)]
        [JsonProperty(PropertyName = "login_terms_of_use")]
    public long? LoginTermsOfUse { get; set; }
  }
}
