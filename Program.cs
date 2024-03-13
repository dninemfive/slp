internal static class Program
{
    internal static TimeOnly StartTime = new(0, 30), EndTime = new(6, 0);
    internal static TimeOnly TimeNow => TimeOnly.FromDateTime(DateTime.Now);
    private static void Main(string[] args)
    {
        if (TimeNow > EndTime || TimeNow < StartTime)
            SleepUntil(StartTime);
    }
    internal static int Minutes(this int minutes)
        => minutes * 60 * 1000;
    private static void SleepUntil(TimeOnly time)
    {
        int delay = (int)(time -  TimeNow).TotalMilliseconds;
        Thread.Sleep(delay);
    }
}