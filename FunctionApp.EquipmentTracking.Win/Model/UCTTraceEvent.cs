using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionApp.EquipmentTracking.Win.Model
{
    public class UCTTraceEvent
    {
        public string Carrier { get; set; }
        public string TFCode { get; set; }
        public string UCTActivityCode { get; set; }
        public string EntityCode { get; set; }
        public string TransportTypeCode { get; set; }
    }

    public class UCTTraceEventEntity : TableEntity
    {
        public string Carrier { get; set; }
        public string TFCode { get; set; }
        public string UCTActivityCode { get; set; }
        public string EntityCode { get; set; }
        public string TransportTypeCode { get; set; }

        public UCTTraceEventEntity()
        {

        }
        public UCTTraceEventEntity(string pk, string rk, UCTTraceEvent ev)
        {
            this.PartitionKey = pk;
            this.RowKey = rk;

            this.TFCode = ev.TFCode;
            this.Carrier = ev.Carrier;
            this.EntityCode = ev.EntityCode;
            this.UCTActivityCode = ev.UCTActivityCode;
            this.EntityCode = ev.EntityCode;
            this.TransportTypeCode = ev.TransportTypeCode;
        }
    }

}
