using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flare.Api.Model
{
    public class DeviceReportResource
    {
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
    }
}
