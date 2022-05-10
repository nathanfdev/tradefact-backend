using System;
using System.Collections.Generic;

namespace Flare.Api.Model
{
    public class DeviceResource
    {
        public Guid Id { get; set; }
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string SerialNumber { get; set; }
        public string IMEI { get; set; }
        public string SimProvider { get; set; }
        public string IMSI { get; set; }
        public string MSISDN { get; set; }
        public bool HasFault { get; set; }
        public DeviceReportResource LastDeviceReport { get; set; }
    }
}
