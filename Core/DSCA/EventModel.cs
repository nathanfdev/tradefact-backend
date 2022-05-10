using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.DSCA
{
    public class EventModel
    {

        /// <summary>
        /// Gets or Sets EventID
        /// </summary>
        [DataMember(Name = "eventID", EmitDefaultValue = false)]
        public Guid EventID { get; set; }

        /// <summary>
        /// Gets or Sets EventDateTime
        /// </summary>
        [DataMember(Name = "eventDateTime", EmitDefaultValue = false)]
        public DateTime EventDateTime { get; set; }

        /// <summary>
        /// Gets or Sets EventClassifierCode
        /// </summary>
        [DataMember(Name = "eventClassifierCode", EmitDefaultValue = false)]
        public string EventClassifierCode { get; set; }

        /// <summary>
        /// Gets or Sets EventTypeCode
        /// </summary>
        [DataMember(Name = "eventTypeCode", EmitDefaultValue = false)]
        public string EventTypeCode { get; set; }

    }
}
