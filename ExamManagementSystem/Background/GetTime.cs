namespace ExamManagementSystem.Background
{
    public static class GetTime
    {
        public static TimeSpan GetDelayTime()
        {
            DateTime now = DateTime.Now;
            DateTime scheduledTime = new(now.Year, now.Month, now.Day, 7, 30, 0);
            if (scheduledTime < now)
            {
                scheduledTime = scheduledTime.AddDays(1);
            }
            TimeSpan timeUntilScheduledTime = scheduledTime - now;
            TimeSpan timeUntilNext1MinuteInterval = TimeSpan.FromMinutes(1) - TimeSpan.FromMinutes(timeUntilScheduledTime.TotalMinutes % 1);
            return timeUntilNext1MinuteInterval;
        }
    }
}
