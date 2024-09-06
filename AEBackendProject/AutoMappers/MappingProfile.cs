using AEBackendProject.DTO.Port;
using AEBackendProject.DTO.Ship;
using AEBackendProject.DTO.User;
using AEBackendProject.Models;
using AutoMapper;

namespace AEBackendProject.AutoMappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region User Entity
            CreateMap<UserCreateDto, User>();
            CreateMap<User, UserDto>();
            #endregion

            #region Ship Entity
            CreateMap<ShipCreateDto, Ship>();
            CreateMap<Ship, ShipDto>();
            #endregion

            #region Port
            CreateMap<PortDistanceTimeDto, PortDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.PortName));
            #endregion
        }
    }
}
