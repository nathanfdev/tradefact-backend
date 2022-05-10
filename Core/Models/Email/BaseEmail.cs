using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class BaseEmail
    {
        [Newtonsoft.Json.JsonProperty("email")]
        public string Email { get; set; }
    }
}
