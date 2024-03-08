using System;
using System.Collections.Generic;

namespace DatabaseScript.Models;

public partial class AuxMovementTug
{
    public ulong IdMovementTugs { get; set; }

    public uint IdMovement { get; set; }

    public uint IdTug { get; set; }
}
