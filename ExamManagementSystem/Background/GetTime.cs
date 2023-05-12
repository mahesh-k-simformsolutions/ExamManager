namespace ExamManagementSystem.Background
{
    public static class GetTime
    {
        public static TimeSpan GetDelayTime()
        {
            var now = DateTime.Now;
            var scheduledTime = new DateTime(now.Year, now.Month, now.Day, 7, 30, 0);
            if (scheduledTime < now)
            {
                scheduledTime = scheduledTime.AddDays(1);
            }
            var timeUntilScheduledTime = scheduledTime - now;
            var timeUntilNext1MinuteInterval = TimeSpan.FromMinutes(1) - TimeSpan.FromMinutes(timeUntilScheduledTime.TotalMinutes % 1);
            return timeUntilNext1MinuteInterval;
        }
    }
}
