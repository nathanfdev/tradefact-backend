using System;
using System.Collections.Generic;
using System.Text;

namespace Integration.ShipsGo.Model
{
    public class ContainerInfo
    {
        public string Message { get; set; }
        public string ReferenceNo { get; set; }
        public string ContainerNumber { get; set; }
        public string FromCountry { get; set; }
        public string Pol { get; set; }
        public string ToCountry { get; set; }
        public string Pod { get; set; }
        public string ShippingLine { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public string FormatedTransitTime { get; set; }
    }

    public class GetContainerInfo
    {
        public string Message { get; set; }
        public string ReferenceNo { get; set; }
        public string ContainerNumber { get; set; }
        public string FromCountry { get; set; }
        public string Pol { get; set; }
        public string ToCountry { get; set; }
        public string Pod { get; set; }
        public string ShippingLine { get; set; }
        public string DepartureDate { get; set; }
        public string ArrivalDate { get; set; }
        public string FormatedTransitTime { get; set; }

        public string RequestStatus { get; set; }
        public int RequestStatusId { get; set; }
        public string SailingStatus { get; set; }
        public int SailingStatusId { get; set; }
        public string BLReferenceNo { get; set; }
        public string ContainerTEU { get; set; }
        public List<TSPortInfo> TSPorts { get; set; }
        public string Vessel { get; set; }
        public string VesselIMO { get; set; }
        public string VesselLatitude { get; set; }
        public string VesselLongitude { get; set; }
        public string GateOutDate { get; set; }
        public string EmptyReturnDate { get; set; }
        public string FinalDeliveryPlace { get; set; }
        public string FinalDeliveryDate { get; set; }
        public string ETA { get; set; }
        public string FirstETA { get; set; }
        public int BLContainerCount { get; set; }
        public List<BLContainerInfo> BLContainers { get; set; }
        public string LiveMapUrl { get; set; }
    }
    public class TSPortInfo
    {
        public string Port { get; set; }
        public string ArrivalDate { get; set; }
        public string DepartureDate { get; set; }
        public string Vessel { get; set; }
        public string VesselIMO { get; set; }

        public string VesselLatitude { get; set; }
        public string VesselLongitude { get; set; }
    }

    public class BLContainerInfo
    {
        public string ContainerCode { get; set; }
        public string ContainerTEU { get; set; }
        public string LiveMapUrl { get; set; }
        public string BLGateOutDate { get; set; }
        public string BLEmptyReturnDate { get; set; }
    }
}
