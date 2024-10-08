using IncidentAlert_Statistics.Models;

namespace IncidentAlert_Statistics.Service.Implementation
{
    public class StatisticsService : IStatisticsService
    {
        // Metoda koja vraća listu rezultata LocationIncidentCount za različite vremenske periode.
        public Dictionary<string, LocationIncidentCount> GetLocationWithMostIncidents(List<IncidentDto> incidents)
        {
            var results = new Dictionary<string, LocationIncidentCount>
            {
                { "Total", CalculateLocationWithMostIncidents(incidents) },
                { "Last year", CalculateLocationWithMostIncidents(incidents, DateTime.Now.AddYears(-1)) },
                { "Last month", CalculateLocationWithMostIncidents(incidents, DateTime.Now.AddMonths(-1)) },
                { "Last week", CalculateLocationWithMostIncidents(incidents, DateTime.Now.AddDays(-7)) }
            };

            return results;
        }

        // Metoda koja vraća listu rezultata LocationCategoryIncidentCount za različite vremenske periode.
        public Dictionary<string, LocationCategoryIncidentCount> GetLocationWithMostIncidentsPerCategory(List<IncidentDto> incidents)
        {
            var results = new Dictionary<string, LocationCategoryIncidentCount>
            {
                { "Total", CalculateLocationWithMostIncidentsPerCategory(incidents) },
                { "Last year", CalculateLocationWithMostIncidentsPerCategory(incidents, DateTime.Now.AddYears(-1)) },
                { "Last month", CalculateLocationWithMostIncidentsPerCategory(incidents, DateTime.Now.AddMonths(-1)) },
                { "Last week", CalculateLocationWithMostIncidentsPerCategory(incidents, DateTime.Now.AddDays(-7)) }
            };

            return results;
        }

        // Metoda koja vraća listu rezultata CategoryIncidentCount za različite vremenske periode.
        public Dictionary<string, List<CategoryIncidentCount>> GetNumberOfIncidentsPerCategory(List<IncidentDto> incidents)
        {
            var results = new Dictionary<string, List<CategoryIncidentCount>>
            {
                { "Total", CalculateNumberOfIncidentsPerCategory(incidents) },
                { "Last year", CalculateNumberOfIncidentsPerCategory(incidents, DateTime.Now.AddYears(-1)) },
                { "Last month", CalculateNumberOfIncidentsPerCategory(incidents, DateTime.Now.AddMonths(-1)) },
                { "Last week", CalculateNumberOfIncidentsPerCategory(incidents, DateTime.Now.AddDays(-7)) }
            };

            return results;
        }

        // Privatne metode za računanje rezultata na osnovu vremenskih perioda.

        private LocationIncidentCount CalculateLocationWithMostIncidents(List<IncidentDto> incidents, DateTime? startDate = null)
        {
            var filteredIncidents = startDate.HasValue
                ? incidents.Where(i => i.DateTime >= startDate.Value)
                : incidents;

            var grouped = filteredIncidents
                .GroupBy(i => i.Location?.Name)
                .Select(g => new LocationIncidentCount
                {
                    LocationName = g.Key ?? "Unknown",
                    IncidentCount = g.Count()
                })
                .OrderByDescending(g => g.IncidentCount)
                .FirstOrDefault();

            return grouped ?? new LocationIncidentCount { LocationName = "Unknown", IncidentCount = 0 };
        }

        private LocationCategoryIncidentCount CalculateLocationWithMostIncidentsPerCategory(List<IncidentDto> incidents, DateTime? startDate = null)
        {
            var filteredIncidents = startDate.HasValue
                ? incidents.Where(i => i.DateTime >= startDate.Value)
                : incidents;

            var grouped = filteredIncidents
                .SelectMany(i => i.Categories, (incident, category) => new { LocationName = incident.Location?.Name, CategoryName = category })
                .GroupBy(x => new { x.LocationName, x.CategoryName })
                .Select(g => new LocationCategoryIncidentCount
                {
                    LocationName = g.Key.LocationName ?? "Unknown",
                    CategoryName = g.Key.CategoryName,
                    IncidentCount = g.Count()
                })
                .OrderByDescending(g => g.IncidentCount)
                .FirstOrDefault();

            return grouped ?? new LocationCategoryIncidentCount { LocationName = "Unknown", CategoryName = "Unknown", IncidentCount = 0 };
        }

        private List<CategoryIncidentCount> CalculateNumberOfIncidentsPerCategory(List<IncidentDto> incidents, DateTime? startDate = null)
        {
            var filteredIncidents = startDate.HasValue
                ? incidents.Where(i => i.DateTime >= startDate.Value)
                : incidents;

            var grouped = filteredIncidents
                .SelectMany(i => i.Categories)
                .GroupBy(c => c)
                .Select(g => new CategoryIncidentCount
                {
                    CategoryName = g.Key,
                    IncidentCount = g.Count()
                })
                .ToList();

            return grouped;
        }
    }
}
