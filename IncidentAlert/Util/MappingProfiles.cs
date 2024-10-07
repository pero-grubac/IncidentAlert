using AutoMapper;
using Contracts.Category;
using Contracts.Incident;
using Contracts.Location;
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
            CreateMap<Incident, ResponseIncidentDto>()
               .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.IncidentCategories.Select(ic => ic.Category)))
               .ReverseMap();
            CreateMap<Location, LocationDto>().ReverseMap();

            // contracts
            CreateMap<Category, CategoryCreatedEvent>().ReverseMap();
            CreateMap<Incident, IncidentCreateEvent>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.IncidentCategories.Select(ic => ic.Category)))
                .ReverseMap();
            CreateMap<Location, LocationCreateEvent>().ReverseMap();
            CreateMap<Incident, IncidentApprovedEvent>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.IncidentCategories.Select(ic => ic.Category)))
                .ReverseMap();
        }
    }
}
