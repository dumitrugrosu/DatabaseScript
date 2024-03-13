using System;
using System.Collections.Generic;

namespace DatabaseScript.Entities;

public partial class AuxBarge
{
    public long Sid { get; set; }

    public long AuxSid { get; set; }

    public long BargeSid { get; set; }

    public virtual AuxManeuver AuxS { get; set; } = null!;

    public virtual MngAuxBarges BargeS { get; set; } = null!;
}
