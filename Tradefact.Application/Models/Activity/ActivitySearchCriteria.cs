using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Application.Models.Activity
{
    public class ActivitySearchCriteria
    {
        public List<ActivityEntityTypeEnum> Entities { get; set; } = new List<ActivityEntityTypeEnum>();
    }
}
