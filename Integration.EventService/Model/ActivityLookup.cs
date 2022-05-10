using Core.Models;

namespace Integration.TradefactActivityService.Model
{
    public class ActivityLookup
    {
        public ActivityTypeEnum Type { get; set; }
        public ActivityEntityTypeEnum Entity { get; set; }
    }
}
