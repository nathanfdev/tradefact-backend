using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionApp.EquipmentTracking.Win.Model
{
    public class TrackTraceEvent
    {
        public string TFCode { get; set; }
        public string EntityCode { get; set; }
        public string Entity { get; set; }
        public string EventTypeCode { get; set; }
        public string EventType { get; set; }
        public string EmptyIndicatorCode { get; set; }
        public string TransportTypeCode { get; set; }
        public string FacilityTypeCode { get; set; }
        public string Location { get; set; }
        public string Event { get; set; }
        public string EventDescription { get; set; }
    }

    public class TrackTraceEventEntity : TableEntity
    {
        public string TFCode { get; set; }
        public string EntityCode { get; set; }
        public string Entity { get; set; }
        public string EventTypeCode { get; set; }
        public string EventType { get; set; }
        public string EmptyIndicatorCode { get; set; }
        public string TransportTypeCode { get; set; }
        public string FacilityTypeCode { get; set; }
        public string Location { get; set; }
        public string Event { get; set; }
        public string EventDescription { get; set; }


        public TrackTraceEventEntity()
        {

        }

        public TrackTraceEventEntity(string pk, string rk, TrackTraceEvent ev)
        {
            this.PartitionKey = pk;
            this.RowKey = rk;

            this.TFCode = ev.TFCode;
            this.EntityCode = ev.EntityCode;
            this.Entity = ev.Entity;
            this.EventTypeCode = ev.EventTypeCode;
            this.EventType = ev.EventType;
            this.EmptyIndicatorCode = ev.EmptyIndicatorCode;
            this.TransportTypeCode = ev.TransportTypeCode;
            this.FacilityTypeCode = ev.FacilityTypeCode;
            this.Location = ev.Location;
            this.Event = ev.Event;
            this.EventDescription = ev.EventDescription;
        }
    }

}
