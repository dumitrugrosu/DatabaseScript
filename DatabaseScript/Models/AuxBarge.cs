using System;
using System.Collections.Generic;

namespace DatabaseScript.Models;

public partial class AuxBarge
{
    public int IdBarge { get; set; }

    public string Barge { get; set; } = null!;

    public int IdUser { get; set; }

    public byte? AdminValidated { get; set; }

    public int? Timestamp { get; set; }
}
