using System;

namespace FunctionApp.EquipmentTracking.Win.Model
{
    public class Results
    {
        public DateTime LastChanged { get; set; }
        public string Status { get; set; }
        public Data Data { get; set; }
    }

}
