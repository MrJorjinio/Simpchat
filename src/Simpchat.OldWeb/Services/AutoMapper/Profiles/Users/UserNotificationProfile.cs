using AutoMapper;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Responses;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserNotificationDtos.Responses;
using System.Linq;

namespace SimpchatWeb.Services.AutoMapper.Profiles.Users
{
    public class UserNotificationProfile : Profile
    {
        public UserNotificationProfile()
        {
            // --- RESPONSES ---
            CreateMap<Notification, UserNotificationGetResponseDto>()
                .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.Message.ChatId))
                .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.MessageId))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Message.Content))
                .ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => src.Message.SentAt))
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Message.Sender.Username))
                .ReverseMap();

            // --- PATCH ---
            CreateMap<UserGetByIdGetResponseDto, Notification>()
                .ForMember(dest => dest.ReceiverId, opt => opt.MapFrom(src => src.Id));
        }
    }
}
