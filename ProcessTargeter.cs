using System.Diagnostics;
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
}