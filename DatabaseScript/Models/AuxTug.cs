using System;
using System.Collections.Generic;

namespace DatabaseScript.Models;

public partial class AuxTug
{
    public uint IdTug { get; set; }

    public string NameTug { get; set; } = null!;

    public uint IdType { get; set; }

    public uint IdFlag { get; set; }

    public uint IdUser { get; set; }

    public sbyte? AdminValidated { get; set; }

    public string? Remark { get; set; }

    public uint? IdUserChange { get; set; }

    public uint? Timestamp { get; set; }
}
