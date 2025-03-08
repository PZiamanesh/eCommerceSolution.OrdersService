using AutoMapper;
using OrdersMicroservice.Core.DTOs;

namespace OrdersMicroservice.Core.MappingObjects;

public class UserDTOToOrderResponseMappingProfile : Profile
{
    public UserDTOToOrderResponseMappingProfile()
    {
        CreateMap<UserDTO, OrderResponse>()
          .ForMember(dest => dest.UserPersonName, opt => opt.MapFrom(src => src.PersonName))
          .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
    }
}