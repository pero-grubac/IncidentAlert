using AutoMapper;
using IncidentAlert.Models;
using IncidentAlert.Models.Dto;

namespace IncidentAlert.Util
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Incident, IncidentDto>().ReverseMap();
            CreateMap<Location, LocationDto>().ReverseMap();
        }
    }
}
