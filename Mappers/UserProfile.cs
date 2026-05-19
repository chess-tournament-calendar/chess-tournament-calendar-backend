using AutoMapper;
using ChessTournamentCalendarBackend.API.DTOs.RequestDTOs;
using ChessTournamentCalendarBackend.API.Entities;

namespace ChessTournamentCalendarBackend.API.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<RegisterRequestDto, User>();
    }
}