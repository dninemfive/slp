using System.Text.Json.Serialization;

namespace die;
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
    public TimeOnly StartTime = TimeOnly.Parse(startTime);
    [JsonInclude]
    public TimeOnly EndTime = TimeOnly.Parse(endTime);
    [JsonInclude]
    public double MinutesBetweenCloseAttempts = minutesBetweenCloseAttempts;
    [JsonInclude]
    public List<ProcessTargeter> Close = close;
    [JsonInclude]
    public List<ProcessTargeter> Allow = allow;
}