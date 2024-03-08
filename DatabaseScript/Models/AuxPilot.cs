using System;
using System.Collections.Generic;

namespace DatabaseScript.Models;

public partial class AuxPilot
{
    public int IdPilot { get; set; }

    public string? Pilot { get; set; }

    public uint? IdUser { get; set; }

    public sbyte? AdminValidated { get; set; }

    public uint? Timestamp { get; set; }
}
