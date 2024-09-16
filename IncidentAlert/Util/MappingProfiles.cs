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
            CreateMap<Incident, IncidentDto>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.IncidentCategories.Select(ic => ic.Category)))
                .ReverseMap();
            CreateMap<Location, LocationDto>().ReverseMap();
        }
    }
}
