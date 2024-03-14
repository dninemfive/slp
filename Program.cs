using d9.utl;
using System.Diagnostics;
internal static class Program
{
    internal static TimeOnly StartTime = new(0, 30), EndTime = new(10, 0);
    internal static TimeOnly TimeNow => TimeOnly.FromDateTime(DateTime.Now);
    internal static int TimeBetweenShutdownAttempts = 10.Minutes();
    private static void Main(string[] args)
    {
        while (true)
        {
            if (TimeNow > EndTime || TimeNow < StartTime)
                SleepUntil(StartTime);
            ShutDownVideoGames();
            SleepFor(TimeBetweenShutdownAttempts);
        }
    }
    internal static int Minutes(this int minutes)
        => minutes * 60 * 1000;
    private static void SleepUntil(TimeOnly time)
    {
        Console.WriteLine($"Sleeping until {time}.");
        int delay = (int)(time -  TimeNow).TotalMilliseconds;
        Thread.Sleep(delay);
    }
    private static void SleepFor(int milliseconds)
    {
        Console.WriteLine($"Sleeping until {TimeOnly.FromDateTime(DateTime.Now + TimeSpan.FromMilliseconds(milliseconds))}.");
        Thread.Sleep(milliseconds);
    }
    private static ProcessModule? TryGetProcessModule(this Process process)
    {
        try
        {
            return process.MainModule;
        }
        catch
        {
            return null;
        }
    }
    internal static bool IsCrashHandler(this Process process)
        => process.ProcessName.Contains("crashhandler", StringComparison.InvariantCultureIgnoreCase);
    internal static bool IsInSteamFolder(this Process process)
        => process.TryGetProcessModule()?
                  .FileName
                  .IsInFolder(@"C:\Program Files (x86)\Steam\steamapps\common") 
            ?? false;
    internal static bool IsVideoGame(this Process process)
        => process.IsInSteamFolder() || process.MainWindowTitle.Contains("Minecraft");
    internal static void ShutDownVideoGames()
    {
        foreach (Process process in Process.GetProcesses())
            if (process.IsVideoGame() && !process.IsCrashHandler())
            {
                Console.WriteLine($"{TimeNow,-10} Closing {process.MainWindowTitle} ({process.ProcessName})...");
                // process.CloseMainWindow();
            }
    }
}