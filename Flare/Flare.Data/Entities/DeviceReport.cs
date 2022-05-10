using Core.Models;
using System;

namespace Flare.Data
{
    public class DeviceReport : BaseEntity<DeviceReport>
    {
        public string DeviceId { get; set; }
        public string ResponseType { get; set; }
        public string ProtocolType { get; set; }
        public int MsgSeqNo { get; set; }
        public double? GpsLatitude { get; set; }
        public double? GpsLongitude { get; set; }
        public string GpsAlarm { get; set; }
        public string GpsStatus { get; set; }
        public bool? GpsIsPrecise { get; set; }
        public double? GpsAltitude { get; set; }
        public double? GpsSpeed { get; set; }
        public double? GpsDirection { get; set; }
        public DateTime? GpsTime { get; set; }
        public DateTime? GpsRecvTime { get; set; }
        public bool? GpsUseLbslocation { get; set; }
        public string GpsAddress { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public int? Battery { get; set; }
        public int CommandMsgSeqNo { get; set; }
        public string CommandId { get; set; }
        public string CommandExecuteResult { get; set; }
        public string ExtraInfo { get; set; }
        public string RawData { get; set; }
    }
}
