using AutoMapper;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Responses;
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
            CreateMap<User, UserSearchResponseDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
            CreateMap<User, UserProfileGetResponseDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
            CreateMap<User, UserSetLastSeenPutDto>();
            CreateMap<User, UserJoinChatResponseDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
            CreateMap<Notification, UserNotificationsResponseDto>()
                .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.Message.ChatId))
                .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.MessageId));
            CreateMap<UserLoginPostDto, User>();
            CreateMap<UserRegisterPostDto, User>();
            CreateMap<UserResponseDto, User>();
            CreateMap<UserPutDto, User>();
            CreateMap<UserSearchResponseDto, User>();
            CreateMap<UserProfileGetResponseDto, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId));
            CreateMap<UserJoinChatResponseDto, User>();
            CreateMap<Notification, UserNotificationMarkAsSeenPutResponseDto>();
            CreateMap<UserSetLastSeenPutDto, Notification>();
            CreateMap<UserNotificationMarkAsSeenPutResponseDto, Notification>();
        }
    }
}
