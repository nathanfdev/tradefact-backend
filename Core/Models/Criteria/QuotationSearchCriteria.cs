using Core.Enums;

namespace Core.Models.Criteria
{
    public class QuotationSearchCriteria : SearchCriteria
    {
        public QuotationStateEnum? Status { get; set; }
        public bool CombineRelatedQuotes { get; set; } = false;
    }
}
