using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseScript.Models;

[Table("aux_tugs")]
public partial class AuxTugs
{
    [Column("id_tug")]
    public int IdTug { get; set; }
    [Column("name_tug")]
    public string NameTug { get; set; } = null!;

    public uint IdType { get; set; }

    public uint IdFlag { get; set; }

    public uint IdUser { get; set; }

    public sbyte? AdminValidated { get; set; }

    public string? Remark { get; set; }

    public uint? IdUserChange { get; set; }

    public uint? Timestamp { get; set; }
}
