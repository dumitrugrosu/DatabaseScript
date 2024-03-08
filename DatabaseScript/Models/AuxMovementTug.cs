using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseScript.Models;

[Table("aux_movement_tugs")]
public partial class AuxMovementTug
{
    [Column("id_movement_tugs")]
    public ulong IdMovementTugs { get; set; }
    [Column("id_movement")]
    public uint IdMovement { get; set; }
    [Column("id_tug")]
    public uint IdTug { get; set; }
}
