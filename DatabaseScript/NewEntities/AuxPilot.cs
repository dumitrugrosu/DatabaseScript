using System;
using System.Collections.Generic;

namespace DatabaseScript.Entities;

public partial class AuxPilot
{
    public long Sid { get; set; }

    public long AuxSid { get; set; }

    public long PilotSid { get; set; }

    public virtual AuxManeuver AuxS { get; set; } = null!;

    public virtual MngAuxPilot PilotS { get; set; } = null!;
}
