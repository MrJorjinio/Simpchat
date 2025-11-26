namespace Simpchat.Application.Extentions
{
    public static class ActivityExtentions
    {
        private static readonly double OnlineThresholdMinutes = 1;

        public static string GetTimeAgo(this DateTimeOffset dateTimeOffset)
        {
            var timeSpan = DateTimeOffset.UtcNow - dateTimeOffset;

            if (timeSpan.TotalMinutes < 1)
                return "Just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} days ago";

            return dateTimeOffset.ToString("MMM dd, yyyy");
        }

        public static bool GetOnlineStatus(this DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.AddMinutes(OnlineThresholdMinutes) > DateTimeOffset.UtcNow;
        }
    }
}
