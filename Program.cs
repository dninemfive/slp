using d9.utl;
using System.Diagnostics;
using System.Text.Json;

namespace d9.slp;
internal static class Program
{
    public const string DefaultConfigPath = "config.json";
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor: initialized in Main
    internal static SlpConfig _config;
#pragma warning restore CS8618
    internal static TimeOnly StartTime => TimeOnly.Parse(_config.StartTime);
    internal static TimeOnly EndTime => TimeOnly.Parse(_config.EndTime);
    internal static TimeOnly TimeNow => TimeOnly.FromDateTime(DateTime.Now);
    internal static int TimeBetweenShutdownAttempts => _config.MinutesBetweenCloseAttempts.ToMilliseconds();
    internal static bool LogToConsole = CommandLineArgs.GetFlag("log");
    private static void Main()
    {
        if(LoadConfig() is SlpConfig config)
        {
            _config = config;
        } 
        else
        {
            return;
        }
        while (true)
        {
            if (TimeNow > EndTime || TimeNow < StartTime)
                SleepUntil(StartTime);
            ClosePrograms();
            SleepFor(TimeBetweenShutdownAttempts);
        }
    }
    private static SlpConfig? LoadConfig()
    {
        string configPath = CommandLineArgs.TryGet(nameof(configPath), CommandLineArgs.Parsers.FilePath) ?? DefaultConfigPath;
        if (Config.TryLoad<SlpConfig>(configPath) is SlpConfig config)
            return config;
        Console.WriteLine($"Failed to load config at {configPath}! Saving an example configuration file there and exiting...");
        File.WriteAllText(configPath, JsonSerializer.Serialize(SlpConfig.Default, Config.DefaultSerializerOptions));
        return null;
    }
    internal static void ClosePrograms()
    {
        foreach (Process process in Process.GetProcesses())
            if (_config.Close.Any(x => x.Matches(process)) && !_config.Allow.Any(x => x.Matches(process)))
            {
                Log($"{TimeNow,-10} Closing {process.MainWindowTitle} ({process.ProcessName})...");
                process.CloseMainWindow();
            }
    }
    private static void SleepUntil(TimeOnly time)
    {
        Log($"Sleeping until {time}.");
        int delay = (int)(time -  TimeNow).TotalMilliseconds;
        Thread.Sleep(delay);
    }
    private static void SleepFor(int milliseconds)
    {
        Log($"Sleeping for {milliseconds}ms until {TimeOnly.FromDateTime(DateTime.Now + TimeSpan.FromMilliseconds(milliseconds))}.");
        Thread.Sleep(milliseconds);
    }
    private static void Log(object? msg)
    {
        if(LogToConsole) Console.WriteLine(msg);
    }
    internal static int ToMilliseconds(this double minutes)
        => (int)(minutes * 60 * 1000);
}