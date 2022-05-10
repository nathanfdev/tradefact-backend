using System.Collections.Generic;

namespace FunctionApp.EquipmentTracking.Win.Model
{
    public class Data
    {
        public DataMeta meta { get; set; }
        public List<Error> errors { get; set; }
        public List<Container> Containers { get; set; }
        public object BookingNumber { get; set; }
        public object BLNumber { get; set; }
    }

    public class Error
    {
        public string error { get; set; }
    }

    public class DataMeta
    {
        public object lastFetch { get; set; }
        public object data_src { get; set; }
        public object queryID { get; set; }
        public object v { get; set; }
        public object lastChanged { get; set; }
        public object NextEventDate { get; set; }
        public bool IsDelivered { get; set; }
    }

}
