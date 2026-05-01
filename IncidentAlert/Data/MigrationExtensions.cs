using IncidentAlert.Models;
using Microsoft.EntityFrameworkCore;

namespace IncidentAlert.Data
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            dataContext.Database.Migrate();
        }

        public static void SeedData(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            if (dataContext.Locations.Any()) return; // already seeded

            var locations = new List<Location>
            {
                new() { Latitude = 44.7722, Longitude = 17.1910, Name = "Banja Luka" },
                new() { Latitude = 43.8563, Longitude = 18.4131, Name = "Sarajevo" },
                new() { Latitude = 44.5377, Longitude = 18.6736, Name = "Tuzla" },
                new() { Latitude = 43.3438, Longitude = 17.8078, Name = "Mostar" },
                new() { Latitude = 45.0000, Longitude = 19.8333, Name = "Beograd" },
            };
            dataContext.Locations.AddRange(locations);

            var categories = new List<Category>
            {
                new() { Name = "Fire" },
                new() { Name = "Flood" },
                new() { Name = "Traffic" },
                new() { Name = "Crime" },
                new() { Name = "Other" },
            };
            dataContext.Categories.AddRange(categories);
            dataContext.SaveChanges();

            var incidents = new List<Incident>
            {
                new() { Title = "Fire in city center", Text = "A fire broke out in the city center.", DateTime = DateTime.UtcNow.AddDays(-10), LocationId = locations[0].Id, IncidentCategories = [new() { CategoryId = categories[0].Id }] },
                new() { Title = "Road flooding", Text = "Heavy flooding on main road.", DateTime = DateTime.UtcNow.AddDays(-9), LocationId = locations[1].Id, IncidentCategories = [new() { CategoryId = categories[1].Id }] },
                new() { Title = "Highway accident", Text = "Multiple vehicle accident on highway.", DateTime = DateTime.UtcNow.AddDays(-8), LocationId = locations[2].Id, IncidentCategories = [new() { CategoryId = categories[2].Id }] },
                new() { Title = "Bank robbery", Text = "Robbery at local bank downtown.", DateTime = DateTime.UtcNow.AddDays(-7), LocationId = locations[3].Id, IncidentCategories = [new() { CategoryId = categories[3].Id }] },
                new() { Title = "Gas leak", Text = "Gas leak in residential building.", DateTime = DateTime.UtcNow.AddDays(-6), LocationId = locations[4].Id, IncidentCategories = [new() { CategoryId = categories[4].Id }] },
                new() { Title = "Forest fire", Text = "Forest fire near national park.", DateTime = DateTime.UtcNow.AddDays(-5), LocationId = locations[1].Id, IncidentCategories = [new() { CategoryId = categories[0].Id }] },
                new() { Title = "Bridge closure", Text = "Bridge closed due to flooding.", DateTime = DateTime.UtcNow.AddDays(-4), LocationId = locations[2].Id, IncidentCategories = [new() { CategoryId = categories[1].Id }] },
            };
            dataContext.Incidents.AddRange(incidents);
            dataContext.SaveChanges();
        }
    }
}
