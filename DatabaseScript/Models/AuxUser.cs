using System;
using System.Collections.Generic;

namespace DatabaseScript.Models;

public partial class AuxUser
{
    public uint IdUser { get; set; }

    public string UserName { get; set; } = null!;

    public string? Pass { get; set; }

    public string UserFullName { get; set; } = null!;

    public byte IsAdmin { get; set; }

    public sbyte? AllowEdit { get; set; }
}
