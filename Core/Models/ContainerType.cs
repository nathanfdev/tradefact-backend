using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Core.Models
{
    public class ContainerType
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string ISOTypeGroup { get; set; }
        public string ISOTypeGroupDescription { get; set; }
        public string Length { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public string AdditionalInformation { get; set; }
        public bool Active { get; set; }
        public bool Air { get; set; }
        public bool Sea { get; set; }
        public bool Road { get; set; }

        [JsonIgnore]
        public string ISOTypeCode { get { return this.Code.Substring(2, 2); } }
        [JsonIgnore]
        public string ISOLengthCode { get { return this.Code.Substring(0, 1); } }
        [JsonIgnore]
        public string ISOSecondSizeCode { get { return this.Code.Substring(1, 1); } }
    }
}
