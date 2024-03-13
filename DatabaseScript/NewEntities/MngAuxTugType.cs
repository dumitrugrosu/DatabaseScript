using System;
using System.Collections.Generic;

namespace DatabaseScript.Entities;

public partial class MngAuxTugType
{
    public long Sid { get; set; }

    public string? Type { get; set; }

    public int? CountFree { get; set; }

    public int? Bunker { get; set; }

    public int? Barge { get; set; }

    public int? Ssn { get; set; }

    public string? Details { get; set; }

    public virtual ICollection<MngAuxTug> MngAuxTugs { get; set; } = new List<MngAuxTug>();
}
