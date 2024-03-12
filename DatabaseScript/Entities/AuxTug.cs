using System;
using System.Collections.Generic;

namespace DatabaseScript.Entities;

public partial class AuxTug
{
    public long Sid { get; set; }

    public long AuxSid { get; set; }

    public long TugSid { get; set; }

    public virtual AuxManeuver AuxS { get; set; } = null!;

    public virtual MngAuxTug TugS { get; set; } = null!;
}
