using System.Text.Json;
using System.Text.Json.Serialization;

namespace die;
[method: JsonConstructor]
public class DieConfig(TimeOnly startTime, TimeOnly endTime, double minutesBetweenCloseAttempts, List<ProcessTargeter> close, List<ProcessTargeter> allow)
{
    public static readonly DieConfig Default = new(new(0, 30),
                                                   new(10),
                                                   10,
                                                   [new(ProcessTargetType.ProcessLocation, @"C:\Program Files (x86)\Steam"),
                                                    new(ProcessTargetType.MainWindowTitle, "Minecraft")],
                                                   [new(ProcessTargetType.ProcessName, "CrashHandler")]);
    [JsonInclude]
    public TimeOnly StartTime { get; private set; } = startTime;
    [JsonInclude]
    public TimeOnly EndTime { get; private set; } = endTime;
    [JsonInclude]
    public double MinutesBetweenCloseAttempts { get; private set; } = minutesBetweenCloseAttempts;
    [JsonInclude]
    public List<ProcessTargeter> Close { get; private set; } = close;
    [JsonInclude]
    public List<ProcessTargeter> Allow { get; private set; } = allow;
    public static DieConfig FromJsonElement(JsonElement jse)
    {
        TimeOnly startTime = TimeOnly.Parse(jse.GetProperty("startTime").GetRawText().Replace("\"", "")),
            endTime = TimeOnly.Parse(jse.GetProperty("endTime").GetRawText().Replace("\"", ""));
        double minutesBetweenCloseAttempts = jse.GetProperty("minutesBetweenCloseAttempts").GetDouble();
        List<ProcessTargeter> close = jse.GetProperty("close").Deserialize<List<JsonElement>>()!.Select(ProcessTargeter.FromJsonElement).ToList(),
            allow = jse.GetProperty("allow").Deserialize<List<JsonElement>>()!.Select(ProcessTargeter.FromJsonElement).ToList();
        return new(startTime, endTime, minutesBetweenCloseAttempts, close, allow);
    }
}