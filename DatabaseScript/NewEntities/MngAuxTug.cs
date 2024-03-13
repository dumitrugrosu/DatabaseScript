using System;
using System.Collections.Generic;

namespace DatabaseScript.Entities;

public partial class MngAuxTug
{
    public long Sid { get; set; }

    public string? TugName { get; set; }

    public string? TugFlagState { get; set; }

    public int? TugStatus { get; set; }

    public int? CountFree { get; set; }

    public int? Bunker { get; set; }

    public int? Barge { get; set; }

    public string? Details { get; set; }

    public long? AuxTypeSid { get; set; }

    public virtual ICollection<AuxTug> AuxTugs { get; set; } = new List<AuxTug>();

    public virtual MngAuxTugType? AuxTypeS { get; set; }
}
