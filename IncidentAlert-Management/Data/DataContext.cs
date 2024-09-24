using IncidentAlert_Management.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IncidentAlert_Management.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Location> Locations { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<IncidentCategory> IncidentCategories { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
               .HasIndex(c => c.Name)
               .IsUnique();

            modelBuilder.Entity<IncidentCategory>()
                .HasKey(ic => new { ic.IncidentId, ic.CategoryId });

            modelBuilder.Entity<IncidentCategory>()
                .HasOne(ic => ic.Incident)
                .WithMany(i => i.IncidentCategories)
                .HasForeignKey(ic => ic.IncidentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IncidentCategory>()
                .HasOne(ic => ic.Category)
                .WithMany(c => c.IncidentCategories)
                .HasForeignKey(ic => ic.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Location>()
                .HasMany(l => l.Incidents)
                .WithOne(i => i.Location)
                .HasForeignKey(i => i.LocationId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Location)
                .WithMany(l => l.Incidents)
                .HasForeignKey(i => i.LocationId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Location>()
                .HasIndex(l => l.Name)
                .IsUnique();

            modelBuilder.Entity<Image>()
                .HasOne<Incident>()
                .WithMany(i => i.Images)
                .HasForeignKey(i => i.IncidentId);
        }
    }
}
