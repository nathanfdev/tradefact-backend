using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Models.Tracking
{
    //[Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EventClassifierCode
    {
        [EnumMember(Value = "1")] PLANNED = 1,
        [EnumMember(Value = "2")] ESTIMATED = 2,
        [EnumMember(Value = "3")] ACTUAL = 3,
    } 


    /// <summary>
    /// The local date and time, where the event took place, in ISO 8601 format.
    /// </summary>
    [DataContract]
    public partial class EventDateTime : IEquatable<EventDateTime>
    {
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class EventDateTime {\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((EventDateTime)obj);
        }

        /// <summary>
        /// Returns true if EventDateTime instances are equal
        /// </summary>
        /// <param name="other">Instance of EventDateTime to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(EventDateTime other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return false;
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                return hashCode;
            }
        }

        #region Operators
#pragma warning disable 1591

        public static bool operator ==(EventDateTime left, EventDateTime right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EventDateTime left, EventDateTime right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591
        #endregion Operators
    }

    /// <summary>
    /// The unique identifier for the Equipment Event ID/Transport Event ID/Shipment Event ID.
    /// </summary>
    [DataContract]
    public partial class EventID : IEquatable<EventID>
    {
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class EventID {\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((EventID)obj);
        }

        /// <summary>
        /// Returns true if EventID instances are equal
        /// </summary>
        /// <param name="other">Instance of EventID to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(EventID other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return false;
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                return hashCode;
            }
        }

        #region Operators
#pragma warning disable 1591

        public static bool operator ==(EventID left, EventID right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EventID left, EventID right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591
        #endregion Operators
    }

    /// <summary>
    /// Unique identifier for Event Type Code.
    /// </summary>
    [DataContract]
    public partial class EventTypeCode : IEquatable<EventTypeCode>
    {
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class EventTypeCode {\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((EventTypeCode)obj);
        }

        /// <summary>
        /// Returns true if EventTypeCode instances are equal
        /// </summary>
        /// <param name="other">Instance of EventTypeCode to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(EventTypeCode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return false;
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                return hashCode;
            }
        }

        #region Operators
#pragma warning disable 1591

        public static bool operator ==(EventTypeCode left, EventTypeCode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EventTypeCode left, EventTypeCode right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591
        #endregion Operators
    }


    [DataContract]
    public partial class ModelEvent : IEquatable<ModelEvent>
    {
        /// <summary>
        /// Gets or Sets EventID
        /// </summary>
        [Required]
        [DataMember(Name = "eventID")]
        public EventID EventID { get; set; }

        /// <summary>
        /// Gets or Sets EventDateTime
        /// </summary>
        [Required]
        [DataMember(Name = "eventDateTime")]
        public EventDateTime EventDateTime { get; set; }

        /// <summary>
        /// Gets or Sets EventClassifierCode
        /// </summary>
        [Required]
        [DataMember(Name = "eventClassifierCode")]
        public EventClassifierCode EventClassifierCode { get; set; }

        /// <summary>
        /// Gets or Sets EventTypeCode
        /// </summary>
        [Required]
        [DataMember(Name = "eventTypeCode")]
        public EventTypeCode EventTypeCode { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ModelEvent {\n");
            sb.Append("  EventID: ").Append(EventID).Append("\n");
            sb.Append("  EventDateTime: ").Append(EventDateTime).Append("\n");
            sb.Append("  EventClassifierCode: ").Append(EventClassifierCode).Append("\n");
            sb.Append("  EventTypeCode: ").Append(EventTypeCode).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ModelEvent)obj);
        }

        /// <summary>
        /// Returns true if ModelEvent instances are equal
        /// </summary>
        /// <param name="other">Instance of ModelEvent to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ModelEvent other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    EventID == other.EventID ||
                    EventID != null &&
                    EventID.Equals(other.EventID)
                ) &&
                (
                    EventDateTime == other.EventDateTime ||
                    EventDateTime != null &&
                    EventDateTime.Equals(other.EventDateTime)
                ) &&
                (
                    EventClassifierCode == other.EventClassifierCode ||
                    EventClassifierCode != null &&
                    EventClassifierCode.Equals(other.EventClassifierCode)
                ) &&
                (
                    EventTypeCode == other.EventTypeCode ||
                    EventTypeCode != null &&
                    EventTypeCode.Equals(other.EventTypeCode)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                if (EventID != null)
                    hashCode = hashCode * 59 + EventID.GetHashCode();
                if (EventDateTime != null)
                    hashCode = hashCode * 59 + EventDateTime.GetHashCode();
                if (EventClassifierCode != null)
                    hashCode = hashCode * 59 + EventClassifierCode.GetHashCode();
                if (EventTypeCode != null)
                    hashCode = hashCode * 59 + EventTypeCode.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
#pragma warning disable 1591

        public static bool operator ==(ModelEvent left, ModelEvent right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ModelEvent left, ModelEvent right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591
        #endregion Operators
    }
}
