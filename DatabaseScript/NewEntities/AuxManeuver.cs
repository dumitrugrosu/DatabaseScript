using System;
using System.Collections.Generic;

namespace DatabaseScript.Entities;

public partial class AuxManeuver
{
    public long Sid { get; set; }

    public int Index { get; set; }

    public DateTime RegisterTime { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }

    public short? PausedSec { get; set; }

    public string? FromPosition { get; set; }

    public string? ToPosition { get; set; }

    public string? FromPort { get; set; }

    public string? ToPort { get; set; }

    public short? ConfirmPortChange { get; set; }

    public DateTime? FromTime { get; set; }

    public DateTime? ToTime { get; set; }

    public DateTime? PausedTime { get; set; }

    public string? Details { get; set; }

    public string? BunkerFieldName { get; set; }

    public string? BunkerFieldImo { get; set; }

    public long? UserSid { get; set; }

    public long? UserEndSid { get; set; }

    public virtual ICollection<AuxBarge> AuxBarges { get; set; } = new List<AuxBarge>();

    public virtual ICollection<AuxPilot> AuxPilots { get; set; } = new List<AuxPilot>();

    public virtual ICollection<AuxTug> AuxTugs { get; set; } = new List<AuxTug>();
}
