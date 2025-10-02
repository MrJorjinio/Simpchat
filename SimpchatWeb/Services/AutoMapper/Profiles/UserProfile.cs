using AutoMapper;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Puts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Responses;

namespace SimpchatWeb.Services.AutoMapper.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserLoginPostDto>();
            CreateMap<User, UserRegisterPostDto>();
            CreateMap<User, UserResponseDto>();
            CreateMap<User, UserPutDto>();
            CreateMap<UserLoginPostDto, User>();
            CreateMap<UserRegisterPostDto, User>();
            CreateMap<UserResponseDto, User>();
            CreateMap<UserPutDto, User>();
        }
    }
}
