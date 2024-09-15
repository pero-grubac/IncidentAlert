using IncidentAlert.Models;
using Microsoft.EntityFrameworkCore;

namespace IncidentAlert.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<Location> Locations { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<IncidentCategory> IncidentCategories { get; set; }

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
                .IsRequired();

        }
        public void Detach<TEntity>(TEntity entity) where TEntity : class
        {
            var entry = this.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                return;
            }
            entry.State = EntityState.Detached;
        }
    }
}
