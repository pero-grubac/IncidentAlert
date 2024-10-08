using IncidentAlert_ML.Model;

namespace IncidentAlert_ML.Service
{
    public interface IIncidentGroupingService
    {
        Task<List<IncidentGroup>> GroupIncidentsByText(List<SimpleIncident> incidents);
    }
}
