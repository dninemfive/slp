using System.Diagnostics;
using System.Text.Json.Serialization;

namespace die;
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