using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace die;
[method: JsonConstructor]
public class ProcessTargeter(ProcessTargetType type, string value)
{
    [JsonInclude]
    public ProcessTargetType TargetType { get; private set; } = type;
    [JsonInclude]
    public string Value { get; private set; } = value;
    public bool Matches(Process t) => TargetType switch
    {
        ProcessTargetType.MainWindowTitle => t.MainWindowTitleContains(Value),
        ProcessTargetType.ProcessName => t.NameContains(Value),
        ProcessTargetType.ProcessLocation => t.IsInFolder(Value),
        _ => throw new Exception($"{TargetType} ({(int)TargetType}) is not a valid process target type!")
    };
    public static ProcessTargeter FromJsonElement(JsonElement jse)
    {
        return new(TypeOf(jse.GetProperty("targetType").GetString()), jse.GetProperty("value").GetString());
    }
    public static ProcessTargetType TypeOf(string s)
    {
        foreach(ProcessTargetType type in Enum.GetValues(typeof(ProcessTargetType)))
        {
            if (type.ToString().ToLower() == s.ToLower())
                return type;
        }
        return (ProcessTargetType)666;
    }
}