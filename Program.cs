using d9.utl;
using System.Diagnostics;
using System.Text.Json.Serialization;
internal static class Program
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor: initialized in Main
    internal static DieConfig _config;
#pragma warning restore CS8618
    internal static TimeOnly StartTime => _config.StartTime;
    internal static TimeOnly EndTime => _config.EndTime;
    internal static TimeOnly TimeNow => TimeOnly.FromDateTime(DateTime.Now);
    internal static int TimeBetweenShutdownAttempts => _config.MinutesBetweenCloseAttempts.ToMilliseconds();
    private static void Main(string[] args)
    {
        _config = Config.TryLoad<DieConfig>("config.json") ?? DieConfig.Default;
        return;
        while (true)
        {
            if (TimeNow > EndTime || TimeNow < StartTime)
                SleepUntil(StartTime);
            ShutDownVideoGames();
            SleepFor(TimeBetweenShutdownAttempts);
        }
    }
    internal static int ToMilliseconds(this int minutes)
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
    internal static bool IsInFolder(this Process process, string folder)
        => process.TryGetProcessModule()?
                  .FileName
                  .IsInFolder(folder)
            ?? false;
    internal static bool NameContains(this Process process, string name)
        => process.ProcessName.Contains(name, StringComparison.InvariantCultureIgnoreCase);
    internal static bool MainWindowTitleContains(this Process process, string name)
        => process.MainWindowTitle.Contains(name, StringComparison.InvariantCultureIgnoreCase);
    internal static void ShutDownVideoGames()
    {
        foreach (Process process in Process.GetProcesses())
            if (process.IsVideoGame() && !process.IsCrashHandler())
            {
                Console.WriteLine($"{TimeNow,-10} Closing {process.MainWindowTitle} ({process.ProcessName})...");
                process.CloseMainWindow();
            }
    }
}
[method: JsonConstructor]
internal class DieConfig(string startTime, string endTime, int minutesBetweenCloseAttempts, List<ProcessTargeter> close, List<ProcessTargeter> allow)
{
    public static DieConfig Default = new("12:30 AM",
                                          "10:00 AM",
                                          10,
                                          [new(ProcessTargetType.ProcessLocation, @"C:\Program Files (x86)\Steam"),
                                           new(ProcessTargetType.MainWindowTitle, "Minecraft")],
                                          [new(ProcessTargetType.ProcessName, "CrashHandler")]);
    [JsonInclude]
    internal TimeOnly StartTime = TimeOnly.Parse(startTime);
    [JsonInclude]
    internal TimeOnly EndTime = TimeOnly.Parse(endTime);
    [JsonInclude]
    internal int MinutesBetweenCloseAttempts = minutesBetweenCloseAttempts;
    [JsonInclude]
    internal List<ProcessTargeter> Close = close;
    [JsonInclude]
    internal List<ProcessTargeter> Allow = allow;
}
[method: JsonConstructor]
internal class ProcessTargeter(ProcessTargetType type, string value)
{
    [JsonInclude]
    public ProcessTargetType TargetType = type;
    [JsonInclude]
    public string Value = value;
    public bool Matches(Process t) => TargetType switch
    {
        ProcessTargetType.MainWindowTitle => t.MainWindowTitleContains(Value),
        ProcessTargetType.ProcessName => t.NameContains(Value),
        ProcessTargetType.ProcessLocation => t.IsInFolder(Value),
        _ => throw new ArgumentOutOfRangeException(nameof(TargetType))
    };
}
internal enum ProcessTargetType
{
    MainWindowTitle, ProcessName, ProcessLocation
}