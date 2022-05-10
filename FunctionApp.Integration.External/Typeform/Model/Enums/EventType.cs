using System.Runtime.Serialization;

namespace FunctionApp.Integration.External.Model.Typeform
{
    public enum EventType
    {
        [EnumMember(Value = "form_response")] FormResponse
    }
}
