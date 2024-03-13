using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseScript.Models;

public partial class AuxPilot
{
    [Column ("id_pilot")]
    public int IdPilot { get; set; }
    [Column ("pilot")]
    public string? Pilot { get; set; }

    public uint? IdUser { get; set; }

    public sbyte? AdminValidated { get; set; }

    public uint? Timestamp { get; set; }
}
