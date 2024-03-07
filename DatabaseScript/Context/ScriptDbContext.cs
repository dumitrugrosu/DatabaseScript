using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseScript.Context
{
    public class ScriptDbContext : DbContext
    {
        public ScriptDbContext(DbContextOptions<ScriptDbContext> options) : base(options)
        {
        }
        public DbSet<Pilot> Pilots { get; set; }
        public DbSet<Tug> Tugs { get; set; }
        public DbSet<Barge> Barges { get; set; }
        public DbSet<MovementTugs> MovementTugs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pilot>().ToTable("aux_pilot");
            modelBuilder.Entity<Tug>().ToTable("aux_tugs");
            modelBuilder.Entity<Barge>().ToTable("aux_barge");
            modelBuilder.Entity<MovementTugs>().HasNoKey();
            modelBuilder.Entity<Movement>().HasNoKey();
        }
    }

    [Table("aux_barge")]
    public class Barge
    {
        [Column("barge")]
        public string? Name { get; set; }
        [Column("id_barge")]
        public int Id { get; set; }
    }

   
    [Table("aux_tugs")]
    public class Tug
    {
        [Column("Name_tug")]
        public string? Name { get; set; }

        [Column("id_tug")]
        public int Id { get; set; }

    }

    [Table("aux_pilot")]
    public class Pilot
    {
        [Column("pilot")]
        public string? Name { get; set; }
        [Column("id_pilot")]
        public int Id { get; set; }
    }

    [Table("aux_movement_tugs")]
    public class MovementTugs
    {
        [Column("id_tug")]
        public int IdTug { get; set; }
    }
    [Table("aux_movement")]
    public class Movement
    {
        [Column("barge_field")]
        public int IdBarge { get; set; }
        [Column("pilot_field")]
        public int IdPilot { get; set; }
    }
}
