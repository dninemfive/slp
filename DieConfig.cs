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
    internal TimeOnly StartTime = TimeOnly.Parse(startTime);
    [JsonInclude]
    internal TimeOnly EndTime = TimeOnly.Parse(endTime);
    [JsonInclude]
    internal double MinutesBetweenCloseAttempts = minutesBetweenCloseAttempts;
    [JsonInclude]
    internal List<ProcessTargeter> Close = close;
    [JsonInclude]
    internal List<ProcessTargeter> Allow = allow;
}