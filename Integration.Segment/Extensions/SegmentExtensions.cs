using Segment.Model;

namespace Integration.Segment.Extensions
{
    public static class SegmentExtensions
    {
        public static Traits AddIfNotNull(this Traits obj, string key, string value)
        {
            if (value != null)
            {
                obj.Add(key, value);
            }

            return obj;
        }
    }
}
