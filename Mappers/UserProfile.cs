using AutoMapper;
using ChessTournamentCalendarBackend.API.DTOs.RequestDTOs;
using ChessTournamentCalendarBackend.API.DTOs.ResponseDTOs;
using ChessTournamentCalendarBackend.API.Entities;

namespace ChessTournamentCalendarBackend.API.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // Auth Service
        CreateMap<RegisterRequestDto, User>();

        // Usr Service
        CreateMap<User, UserResponseDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title.ToString()));

        CreateMap<UpdateProfileRequestDto, User>()
            .ForMember(dest => dest.Name, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Name)));
    }
}