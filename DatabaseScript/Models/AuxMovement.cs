using System;
using System.Collections.Generic;

namespace DatabaseScript.Models;

public partial class AuxMovement
{
    public uint IdMovement { get; set; }

    public uint IdFrom { get; set; }

    public uint IdTo { get; set; }

    public uint StartTime { get; set; }

    public uint StopTime { get; set; }

    public string? BunkerField { get; set; }

    public uint PilotField { get; set; }

    public uint? BargeField { get; set; }

    public string? RemarkField { get; set; }

    public sbyte? Paused { get; set; }

    public long Signature { get; set; }

    public uint IdUserStart { get; set; }

    public uint IdUserStop { get; set; }
}
