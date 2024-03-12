using System;
using System.Collections.Generic;

namespace DatabaseScript.Entities;

public partial class MngAuxBarges
{
    public long Sid { get; set; }

    public string? BargeName { get; set; }

    public int? MmsiNumber { get; set; }

    public int? BargeStatus { get; set; }

    public string? Details { get; set; }

    public virtual ICollection<AuxBarge> AuxBarges { get; set; } = new List<AuxBarge>();
}
