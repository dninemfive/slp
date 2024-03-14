# slp - <ins>Sl</ins>ee<ins>p</ins> enforcement tool.
A simple script to manage my productivity by shutting down programs which keep me awake too late or distract me in the morning.

Uses [utl](https://github.com/dninemfive/utl) to parse a JSON config file, which has the following elements:
- `startTime`: the earliest time in the day that the script should try to close the relevant programs.
- `endTime`: the latest time in the day that the script should try to close the relevant programs.
- `minutesBetweenCloseAttempts`: the time, in minutes, the script will wait between each attempt at closing the relevant programs.
- `close` and `allow`: two lists with the following elements, which specify the programs to close or allow, respectively. `allow` overrides `close` in priority.
  - `targetType`: One of `processLocation`, `processName`, or `mainWindowTitle`, specifying which property on each process the rule checks.
  - `value`: a string the rule will check for in the specified property. The comparison is **case-insensitive** and will return true if the string is a **substring** of the specified property.
  - `startLateAt`: *[optional]* A time, later than `startTime`, before which the specific rule will not be applied. If `startLateAt` is earlier than `startTime`, the rule will only be applied after `startTime`, for performance reasons.
  - `endEarlyAt`: *[optional]* A time, earlier than `endTime`, before which the specific rule will not be applied. If `endEarlyAt` is later than `endTime`, the rule will only be applied before `endTime`, for performance reasons.

See [exampleConfig.json](https://github.com/dninemfive/slp/blob/main/exampleConfig.json) for an example.