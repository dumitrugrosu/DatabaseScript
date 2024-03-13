using System;
using System.Collections.Generic;

namespace DatabaseScript.Models;

public partial class AuxType
{
    public uint IdType { get; set; }

    public string Type { get; set; } = null!;

    public sbyte? CountEvenFreeRunning { get; set; }

    public sbyte? BunkerField { get; set; }

    public sbyte? BargeField { get; set; }

    public sbyte? PilotField { get; set; }

    public sbyte? SsnRequested { get; set; }
}
