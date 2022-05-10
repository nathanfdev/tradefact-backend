using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Core.Models
{

    public class Message: BaseEntity<Message>
    {
        public Message()
        {
            Starred = new List<User>();
            Attachments = new List<Attachment>();
        }
        
        public Guid RoomId { get; set; }

        public Room Room { get; set; }

        public string Comment { get; set; }

        public bool IsPinned { get; set; }

        public DateTime? PinnedAt { get; set; }

        [JsonIgnore]
        public User PinnedBy { get; set; }

        public User PostedBy { get; set; }

        //[JsonIgnore]
        //public Guid PostedByUserId { get; set; }

        //[JsonProperty("postedBy")]
        //public virtual UserProfile User { get; set; }

        public bool WasEdited => this.LastModifiedOn != null;

        public string Type { get; set; }
        public List<Reaction> Reactions { get; set; }
        public List<User> Starred { get; set; }

        public List<Attachment> Attachments { get; set; }


        public override string ToString()
        {
            return $"{this.CreatedByUser ?? "UnknownUser"}: {Comment}";
        }

        private bool Equals(Message other)
        {
            return string.Equals(Id, other.Id) && string.Equals(RoomId, other.RoomId) &&
                   string.Equals(Comment, other.Comment)  && Equals(CreatedByUser, other.CreatedByUser) &&
                   CreationDate.Equals(other.CreationDate) && Equals(LastChangeUser, other.LastChangeUser) &&
                   LastModifiedOn.Equals(other.LastModifiedOn) && string.Equals(Type, other.Type) && Starred.Equals(other.Starred);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Message)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 0;
                hashCode = (hashCode * 397) ^ (Comment?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (CreatedByUser?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ CreationDate.GetHashCode();
                hashCode = (hashCode * 397) ^ (LastChangeUser?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ LastModifiedOn.GetHashCode();
                hashCode = (hashCode * 397) ^ (Type?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ Starred.GetHashCode();
                return hashCode;
            }
        }
    }
}