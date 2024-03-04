using Microsoft.EntityFrameworkCore;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pilot>();
            modelBuilder.Entity<Tug>();
            modelBuilder.Entity<Barge>();
        }
    }

    public class Barge
    {
        public int Primary { get; set; }
    }

    public class Tug
    {
        public int Primary { get; set; }
        public string? Name { get; set; }
        public int Id { get; set; }
    }

    public class Pilot
    {
        public int Primary { get; set; }
    }
}
