using System.Collections.Generic;

namespace Core.Models.Common
{
    public partial class DynamicPropertyDictionaryItem : Entity
    {
        public string PropertyId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public IList<string> DisplayNames { get; set; } = new List<string>();
    }


}
