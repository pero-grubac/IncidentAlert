using IncidentAlert_Statistics.Models;

namespace IncidentAlert_Statistics.Service
{
    public interface IStatisticsService
    {
        // Metoda koja vraća listu rezultata LocationIncidentCount za različite vremenske periode.
        Dictionary<string, LocationIncidentCount> GetLocationWithMostIncidents(List<IncidentDto> incidents);

        // Metoda koja vraća listu rezultata LocationCategoryIncidentCount za različite vremenske periode.
        Dictionary<string, LocationCategoryIncidentCount> GetLocationWithMostIncidentsPerCategory(List<IncidentDto> incidents);

        // Metoda koja vraća listu rezultata CategoryIncidentCount za različite vremenske periode.
        Dictionary<string, List<CategoryIncidentCount>> GetNumberOfIncidentsPerCategory(List<IncidentDto> incidents);

        /// <summary>Incidents per place with coordinates, busiest first. Only incidents since <paramref name="since"/> when given.</summary>
        List<LocationHeat> GetLocationHeat(List<IncidentDto> incidents, DateTime? since);

        /// <summary>Incidents per day for the <paramref name="days"/> days up to and including <paramref name="today"/>.</summary>
        List<DailyIncidentCount> GetIncidentsPerDay(List<IncidentDto> incidents, int days, DateOnly today);
    }
}
