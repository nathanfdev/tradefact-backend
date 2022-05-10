using System;
using System.Collections.Generic;
using Tradefact.Application.Models;

namespace Tradefact.Application.Shipments
{
    public class ShipmentExceptionsInfo
    {
        public List<string> GetShipmentExceptions(CoreExceptionsInfo shipment)
        {
            List<string> except = new List<string>();
            DateTime now = DateTime.Now;
            if (shipment.ShipmentType == 2)
            {
                now = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            }

            if (shipment.Delivered)
            {
                return except;
            }

            if (shipment.EstimatedCollectionDate < now && !shipment.Collected)
            {
                except.Add("Collection date reached");
            }
            if (shipment.EstimatedDeparturePOL < now && !shipment.DepartedPOL)
            {
                except.Add("Departure date reached. Tracking not updated");
            }
            if (shipment.EstimatedArrivalPOD < now && !shipment.ArrivedPOD)
            {
                except.Add("Arrival date reached. Tracking not updated");
            }
            if (shipment.IssueAtCustoms && !shipment.CustomsClearence) except.Add("Delay at customs");
            if (shipment.EstimatedDeliveryDate < now) except.Add("Delivery date reached");
            return except;
        }
    }

}