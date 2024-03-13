using System;
using System.Collections.Generic;

namespace DatabaseScript.Models;

public partial class AuxWarn
{
    public uint WarnId { get; set; }

    public string Message { get; set; } = null!;

    public uint Time { get; set; }

    public sbyte WarnType { get; set; }

    public int GeneratedByUser { get; set; }

    public int? ClearedByUser { get; set; }

    public bool Erased { get; set; }
}
