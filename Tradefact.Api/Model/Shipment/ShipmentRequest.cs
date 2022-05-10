using Core.Attributes;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tradefact.Api.Model.Shipment
{
    [TypescriptAutoGeneration]
    public class ShipmentRequest
    {
        public Guid ShipmentId { get; set; }
    }

    public class AssignCollectionDateRequest : ShipmentRequest
    {
        public DateTime CollectionDate { get; set; }
    }

    public class RecordCollectionRequest : ShipmentRequest
    {
        public bool Collected { get; set; }
        public DateTime CollectionDate { get; set; }
    }

    public class RecordDepartedPOLRequest : ShipmentRequest
    {
        public bool Collected { get; set; }
        public DateTime DepartureDatePOL { get; set; }
    }

    public class RecordArrivedPODRequest : ShipmentRequest
    {
        public bool Arrived { get; set; }
        public DateTime ArrivalDatePOD { get; set; }
    }

    public class RecordIssueAtCustomsRequest : ShipmentRequest
    {
        public bool IssueAtCustoms { get; set; }
        public DateTime IssueAtCustomsDate { get; set; }
    }

    public class RecordClearedCustomsRequest : ShipmentRequest
    {
        public bool ClearedCustoms { get; set; }
        public DateTime ClearedCustomsDate { get; set; }
    }

    public class RecordEstimatedDeliveryDateRequest : ShipmentRequest
    {
        public DateTime EstimatedDeliveryDate { get; set; }
    }

    public class RecordDeliveryCompleteRequest : ShipmentRequest
    {
        public bool Delivered { get; set; }
        public DateTime DeliveryDate { get; set; }
    }

    [TypescriptAutoGeneration]
    public class AssignTrackingInformationRequest : ShipmentRequest
    {
        public string Vessel { get; set; }
        public string VesselIMO { get; set; }
        public string BolNumber { get; set; }
        public string Carrier { get; set; }
        public List<string> ContainerIds { get; set; }
    }

    public class AssignTrackingInformationRequestValidator : AbstractValidator<AssignTrackingInformationRequest>
    {

        public AssignTrackingInformationRequestValidator()
        {

            RuleForEach(x => x.ContainerIds).SetValidator(new ContainerIdValidator());
        }

        const string regex = @"^([A-Z]{3})([U,J,Z]{1})([0-9]{7})";
        const string alphas = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private class ContainerIdValidator : AbstractValidator<string>
        {
            private bool IsValidContainerId(string containerId)
            {
                Dictionary<char, int> _equivalentNumericalValues = new Dictionary<char, int>();
                int numerical = 10;
                foreach (char alpha in alphas.ToCharArray())
                {
                    // 11 and multiples thereof are omitted
                    if (numerical % 11 == 0) numerical++;
                    _equivalentNumericalValues.Add(alpha, numerical);
                    numerical++;
                }


                Match match = Regex.Match(containerId, regex, RegexOptions.IgnoreCase);
                if (!match.Success) return false;

                string checkDigit = containerId.Substring(containerId.Length - 1);
                double check_total = 0;
                int index = 0;

                foreach (char positional_char in containerId.Substring(index, 4).ToCharArray())
                {
                    int equivalentNumerical = _equivalentNumericalValues[positional_char];
                    check_total = check_total + equivalentNumerical * (Math.Pow(2, index));
                    index++;
                }

                foreach (int positional_int in containerId.Substring(index, 6).ToCharArray().Select(x => x - '0'))
                {
                    check_total = check_total + positional_int * (Math.Pow(2, index));
                    index++;
                }

                int calcCheckDigit = (int)(check_total - (Math.Floor(check_total / 11) * 11));

                return (calcCheckDigit.ToString() == checkDigit);
            }

            public ContainerIdValidator()
            {
                RuleFor(x => x)
                .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
                .Length(2, 25)
                .Must(IsValidContainerId).WithMessage("{PropertyName} .");
            }
        }

    }



}
