using NodaTime;

namespace Core.Models
{
    public class DateTimeGoodsReady
    {
        // 1. Receive Time Zone from client: Intl.DateTimeFormat().resolvedOptions().timeZone
        private static IClock _clock;

        public DateTimeGoodsReady()
        {

        }

        public DateTimeGoodsReady(string timeZone, int from, int to, IClock clock)
        {
            _clock = clock;

            TimeZone = timeZone;
            Offset = ConvertFromTimeZoneToMinutesOffset(timeZone);
            From = from;
            To = to;
        }

        private static int ConvertFromTimeZoneToMinutesOffset(string timeZone)
        {
            var zone = DateTimeZoneProviders.Tzdb[timeZone];
            var offset = zone.GetUtcOffset(SystemClock.Instance.GetCurrentInstant());
            //_clock.GetCurrentInstant());
            return offset.Milliseconds / NodaConstants.MillisecondsPerMinute;
        }

        public int From { get; }

        public int Offset { get; }

        public string TimeZone { get; }

        public int To { get; }
    }
}