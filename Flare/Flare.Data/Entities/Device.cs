using Core.Models;
using System;

namespace Flare.Data
{
    public class Device : BaseEntity<Device>
    {
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string SerialNumber { get; set; }
        public string IMEI { get; set; }
        public string SimProvider { get; set; }
        public string IMSI { get; set; }
        public string MSISDN { get; set; }
        public bool HasFault { get; set; }
        public Guid LastDeviceReportId { get; set; }
        public DeviceReport LastDeviceReport { get; set; }
    }
}
