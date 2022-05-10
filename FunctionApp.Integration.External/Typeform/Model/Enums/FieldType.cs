using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace FunctionApp.Integration.External.Model.Typeform
{
    public enum FieldType
    {
        [EnumMember(Value = "date")]
        Date,

        [EnumMember(Value = "dropdown")]
        Dropdown,

        [EnumMember(Value = "email")]
        Email,

        [EnumMember(Value = "phone_number")]
        PhoneNumber,

        [EnumMember(Value = "file_upload")]
        FileUpload,

        [EnumMember(Value = "group")]
        Group,

        [EnumMember(Value = "legal")]
        Legal,

        [EnumMember(Value = "long_text")]
        LongText,

        [EnumMember(Value = "multiple_choice")]
        MultipleChoice,

        [EnumMember(Value = "number")]
        Number,

        [EnumMember(Value = "opinion_scale")]
        OpinionScale,

        [EnumMember(Value = "payment")]
        Payment,

        [EnumMember(Value = "picture_choice")]
        PictureChoice,

        [EnumMember(Value = "rating")]
        Rating,

        [EnumMember(Value = "short_text")]
        ShortText,

        [EnumMember(Value = "statement")]
        Statement,

        [EnumMember(Value = "website")]
        Website,

        [EnumMember(Value = "yes_no")]
        YesNo
    }
}
