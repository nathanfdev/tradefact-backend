using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Text;

namespace Core.Models
{

    public interface IEntity
    {
        Guid Id { get; set; }

        bool IsActive { get; set; }
    }

    public interface ITrackableEntity
    {
        string CreatedByUser { get; set; }

        DateTimeOffset CreationDate { get; set; }

        string LastChangeUser { get; set; }

        DateTimeOffset LastModifiedOn { get; set; }
    }

    public abstract class Entity : IEntity
    {
        [JsonProperty(Order = -900)]
        public Guid Id { get; set; }

        [JsonProperty(Order = -995)]
        public bool IsActive { get; set; } = true;
    }

    public abstract class BaseEntity<T>:  Entity, ITrackableEntity where T : class
    {
        //[JsonProperty(Order = -994)]
        //public Guid Id { get; set; }

        [JsonIgnore]
        public string CreatedByUser { get; set; }

        public DateTime CreationDateInternal { get; set; }

        public string LastChangeUser { get; set; }

        public DateTime LastModifiedOnInternal { get; set; }

        [JsonIgnore]
        public byte[] Timestamp { get; set; }

        //public static class BaseEntityExpressions
        //{
        //    public static readonly Expression<Func<BaseEntity<T>, DateTime>> CreationDate = c => c.CreationDateInternal;
        //    public static readonly Expression<Func<BaseEntity<T>, DateTime>> LastModifiedOn = c => c.LastModifiedOnInternal;
        //}

        // Other properties

        public DateTimeOffset LastModifiedOn
        {
            get { return new DateTimeOffset(LastModifiedOnInternal); }
            set { LastModifiedOnInternal = value.DateTime; }
        }

        [JsonIgnore]
        public DateTimeOffset CreationDate
        {
            get { return new DateTimeOffset(CreationDateInternal); }
            set { CreationDateInternal = value.DateTime; }
        }

    }
}
