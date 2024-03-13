using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseScript.Models;

public partial class AuxBarge
{
    [Column("id_barge")]
    public int IdBarge { get; set; }
    [Column("barge")]
    public string Barge { get; set; } = null!;

    public int IdUser { get; set; }

    public byte? AdminValidated { get; set; }

    public int? Timestamp { get; set; }
}
