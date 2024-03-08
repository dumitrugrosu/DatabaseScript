using System;
using System.Collections.Generic;

namespace DatabaseScript.Models;

public partial class AuxEstimatedTime
{
    public int IdAux { get; set; }

    public int A { get; set; }

    public int B { get; set; }

    public bool Unit { get; set; }

    public long SumTime { get; set; }

    public int SumMan { get; set; }

    public long LastStamp { get; set; }
}
