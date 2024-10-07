using AutoMapper;
using Contracts.Category;
using Contracts.Incident;
using Contracts.Location;
using IncidentAlert_Management.Models;
using IncidentAlert_Management.Models.Dto;

namespace IncidentAlert_Management.Util
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Incident, IncidentDto>()
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(src => src.IncidentCategories.Select(ic => ic.Category)))
                .ReverseMap();
            CreateMap<Incident, ResponseIncidentDto>()
               .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.IncidentCategories.Select(ic => ic.Category)))
               .ReverseMap();
            CreateMap<Location, LocationDto>().ReverseMap();
            CreateMap<ApplicationUser, UserDto>().ReverseMap();

            // contracts
            CreateMap<Category, CategoryCreatedEvent>().ReverseMap();
            CreateMap<Incident, IncidentCreateEvent>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.IncidentCategories.Select(ic => ic.Category)))
                .ReverseMap();
            CreateMap<Location, LocationCreateEvent>().ReverseMap();
        }
    }
}
