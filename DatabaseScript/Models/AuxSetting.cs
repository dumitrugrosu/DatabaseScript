using System;
using System.Collections.Generic;

namespace DatabaseScript.Models;

public partial class AuxSetting
{
    public int SettingId { get; set; }

    public string SettingName { get; set; } = null!;

    public string? SettingValue { get; set; }

    public string? SettingExplicatie { get; set; }
}
