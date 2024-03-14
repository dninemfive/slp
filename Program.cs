using d9.utl;
using System.Diagnostics;
using System.Text.Json;

namespace die;
internal static class Program
{
    public const string DefaultConfigPath = "config.json";
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor: initialized in Main
    internal static DieConfig _config;
#pragma warning restore CS8618
    internal static TimeOnly StartTime => _config.StartTime;
    internal static TimeOnly EndTime => _config.EndTime;
    internal static TimeOnly TimeNow => TimeOnly.FromDateTime(DateTime.Now);
    internal static int TimeBetweenShutdownAttempts => _config.MinutesBetweenCloseAttempts.ToMilliseconds();
    private static void Main()
    {
        LoadConfig();
        while (true)
        {
            if (TimeNow > EndTime || TimeNow < StartTime)
                SleepUntil(StartTime);
            ClosePrograms();
            SleepFor(TimeBetweenShutdownAttempts);
        }
    }
    private static void LoadConfig()
    {
        string configPath = CommandLineArgs.TryGet(nameof(configPath), CommandLineArgs.Parsers.FilePath) ?? DefaultConfigPath;
        DieConfig? config = Config.TryLoad<DieConfig>(configPath);
        if (config is null)
        {
            Console.WriteLine($"Failed to load config at {configPath}, saving defaults there...");
            config = DieConfig.Default;
            File.WriteAllText(configPath, JsonSerializer.Serialize(config, Config.DefaultSerializerOptions));
        }
        _config = config!;
    }
    internal static void ClosePrograms()
    {
        foreach (Process process in Process.GetProcesses())
            if (_config.Close.Any(x => x.Matches(process)) && !_config.Allow.Any(x => x.Matches(process)))
            {
                Console.WriteLine($"{TimeNow,-10} Closing {process.MainWindowTitle} ({process.ProcessName})...");
                process.CloseMainWindow();
            }
    }
    private static void SleepUntil(TimeOnly time)
    {
        Console.WriteLine($"Sleeping until {time}.");
        int delay = (int)(time -  TimeNow).TotalMilliseconds;
        Thread.Sleep(delay);
    }
    private static void SleepFor(int milliseconds)
    {
        Console.WriteLine($"Sleeping for {milliseconds}ms until {TimeOnly.FromDateTime(DateTime.Now + TimeSpan.FromMilliseconds(milliseconds))}.");
        Thread.Sleep(milliseconds);
    }
    internal static int ToMilliseconds(this double minutes)
        => (int)(minutes * 60 * 1000);
}