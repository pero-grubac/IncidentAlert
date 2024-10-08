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
            CreateMap<Category, CategoryUpdateEvent>().ReverseMap();
            CreateMap<Category, CategoryDeleteEvent>().ReverseMap();
            CreateMap<Category, CategoryCreatedEvent>().ReverseMap();

            CreateMap<Incident, IncidentCreateEvent>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.IncidentCategories.Select(ic => ic.Category)))
                .ReverseMap();
            CreateMap<Incident, IncidentApprovedEvent>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.IncidentCategories.Select(ic => ic.Category)))
                .ReverseMap();
            CreateMap<IncidentDto, IncidentCreateEvent>().ReverseMap();
            CreateMap<IncidentDto, IncidentApprovedEvent>().ReverseMap();

            CreateMap<Location, LocationCreateEvent>().ReverseMap();
            CreateMap<LocationDto, LocationCreateEvent>().ReverseMap();

            CreateMap<Contracts.Image.ImageData, ImageData>().ReverseMap();

        }
    }
}
