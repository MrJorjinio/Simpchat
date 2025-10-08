using AutoMapper;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Responses;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Puts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Responses;

namespace SimpchatWeb.Services.AutoMapper.Profiles.Users
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // --- POSTS ---
            CreateMap<User, UserLoginPostDto>();
            CreateMap<UserLoginPostDto, User>();

            CreateMap<User, UserRegisterPostDto>();
            CreateMap<UserRegisterPostDto, User>();

            CreateMap<User, UserGlobalRolesPostDto>();
            CreateMap<UserGlobalRolesPostDto, User>();

            // --- PATCH ---
            CreateMap<User, UserProfilePutDto>();
            CreateMap<UserProfilePutDto, User>();

            CreateMap<User, UserPutPasswordDto>();
            CreateMap<UserPutPasswordDto, User>();

            // --- RESPONSES ---
            CreateMap<User, UserGetByIdGetResponseDto>()
                .ReverseMap();

            CreateMap<User, UserSearchResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            CreateMap<UserSearchResponseDto, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<User, UserGetByIdGetResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsOnline, opt => opt.MapFrom(src => (DateTimeOffset.UtcNow - src.LastSeen).TotalSeconds < 5))
                .ReverseMap();

            CreateMap<User, UserJoinChatResponseDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();
        }
    }
}
