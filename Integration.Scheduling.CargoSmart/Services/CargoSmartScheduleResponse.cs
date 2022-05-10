using System;
using System.Collections.Generic;
using System.Text;

namespace Integration.CargoSmart
{

    public class CargoSmartScheduleResponse
    {
        public DateRange ateRange { get; set; }
        public string requestRefNo { get; set; }
        public List<CarrierRoute> routeGroupsList { get; set; }
    }


    public class DateRange
    {
        public DateTime departureFrom { get; set; }
        public DateTime departureTo { get; set; }
    }

    public class CarrierRoute
    {
        public Identification identification { get; set; }
        public Carrier carrier { get; set; }
        public Port por { get; set; }
        public Port fnd { get; set; }
        public List<Route> route { get; set; }
    }

    public class Carrier
    {
        public int carrierID { get; set; }
        public string scac { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string shortName { get; set; }
    }

    public class Identification
    {
        public string dataSourceType { get; set; }
        public string requestRefNo { get; set; }
    }

    public class Port
    {
        public Location location { get; set; }
        public DateTime? eta { get; set; }
        public DateTime? etd { get; set; }
    }


    public class Location
    {
        public string _id { get; set; }
        public string source { get; set; }
        public string unlocode { get; set; }
        public string name { get; set; }
        public string uc_name { get; set; }
        public List<double> geo { get; set; }
        public string locationID { get; set; }
        public int csID { get; set; }
        public int csCityID { get; set; }
        public string type { get; set; }
        public string fullName { get; set; }
        public string timezone { get; set; }
        public DateTime refreshDateTime { get; set; }
    }

    public class DefaultCutoff
    {
        public DateTime cutoffTime { get; set; }
    }

    public class Facility
    {
        public string code { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string type { get; set; }
    }

    public class FromPoint
    {
        public Location location { get; set; }
        public DateTime defaultCutoff { get; set; }
        public DateTime? etd { get; set; }
    }

    public class ToPoint
    {
        public Location location { get; set; }
        public DateTime? eta { get; set; }
    }

    public class Service
    {
        public int serviceID { get; set; }
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Vessel
    {
        public string vesselGID { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public int IMO { get; set; }
    }

    public class Leg
    {
        public FromPoint fromPoint { get; set; }
        public ToPoint toPoint { get; set; }
        public string transportMode { get; set; }
        public Service service { get; set; }
        public Vessel vessel { get; set; }
        public string externalVoyageNumber { get; set; }
        public int transitTime { get; set; }
    }

    public class Route
    {
        public long csRouteID { get; set; }
        public long csPointPairID { get; set; }
        public string carrierScac { get; set; }
        public DateTime touchTime { get; set; }
        public DefaultCutoff defaultCutoff { get; set; }
        public Port por { get; set; }
        public Port fnd { get; set; }
        public bool direct { get; set; }
        public string importHaulage { get; set; }
        public string exportHaulage { get; set; }
        public int transitTime { get; set; }
        public bool isPossibleDirect { get; set; }
        public bool isUncertainTransitTime { get; set; }
        public List<Leg> leg { get; set; }
    }
}
