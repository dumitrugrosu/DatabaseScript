using System;
using System.Collections.Generic;

namespace DatabaseScript.Entities;

public partial class MngAuxPilot
{
    public long Sid { get; set; }

    public string? PilotName { get; set; }

    public int? PilotStatus { get; set; }

    public string? Details { get; set; }

    public string? Category { get; set; }

    public long? CompanySid { get; set; }

    public virtual ICollection<AuxPilot> AuxPilots { get; set; } = new List<AuxPilot>();
}
