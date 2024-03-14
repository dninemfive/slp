using System.Text.Json;
using System.Text.Json.Serialization;

namespace die;
[method: JsonConstructor]
public class DieConfig(string startTime, string endTime, double minutesBetweenCloseAttempts, List<ProcessTargeter> close, List<ProcessTargeter> allow)
{
    public static readonly DieConfig Default = new("12:30 AM",
                                                   "10:00 AM",
                                                   10,
                                                   [new(ProcessTargetType.ProcessLocation, @"C:\Program Files (x86)\Steam"),
                                                    new(ProcessTargetType.MainWindowTitle, "Minecraft")],
                                                   [new(ProcessTargetType.ProcessName, "CrashHandler")]);
    [JsonInclude]
    public TimeOnly StartTime { get; private set; } = TimeOnly.Parse(startTime);
    [JsonInclude]
    public TimeOnly EndTime { get; private set; } = TimeOnly.Parse(endTime);
    [JsonInclude]
    public double MinutesBetweenCloseAttempts { get; private set; } = minutesBetweenCloseAttempts;
    [JsonInclude]
    public List<ProcessTargeter> Close { get; private set; } = close;
    [JsonInclude]
    public List<ProcessTargeter> Allow { get; private set; } = allow;
}