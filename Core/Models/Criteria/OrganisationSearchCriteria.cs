using Core.Enums;

namespace Core.Models.Criteria
{
    public class OrganisationSearchCriteria : SearchCriteria
    {
        public ConfirmedUsersStatusEnum? Status { get; set; }

        public bool OrderByName { get; set; }

        public bool ShowPending { get; set; } = true;

        public string CountryCode { get; set; } = null;
    }
}
