using System;
using System.Collections.Generic;

namespace DatabaseScript.Models;

public partial class AuxFlag
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Alpha2 { get; set; } = null!;

    public string Alpha3 { get; set; } = null!;
}
