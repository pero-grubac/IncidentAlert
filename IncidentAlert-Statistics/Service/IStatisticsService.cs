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
    }
}
