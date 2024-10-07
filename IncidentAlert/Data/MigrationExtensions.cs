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
    }
}
