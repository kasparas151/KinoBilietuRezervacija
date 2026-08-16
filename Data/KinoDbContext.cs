using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using KinoBilietuRezervacija.Models;
using Microsoft.EntityFrameworkCore;

namespace KinoBilietuRezervacija.Data
{
    public class KinoDbContext : IdentityDbContext<ApplicationUser>
    {
        public KinoDbContext(DbContextOptions<KinoDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Screening> Screening { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie>().ToTable("movies");
            modelBuilder.Entity<Screening>().ToTable("screenings");
            modelBuilder.Entity<Ticket>().ToTable("tickets");

            modelBuilder.Entity<Screening>()
                .HasOne(s => s.Filmas)
                .WithMany()
                .HasForeignKey(s => s.FilmoID);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Seansas)
                .WithMany()
                .HasForeignKey(t => t.SeansoID);
        }
    }
}
