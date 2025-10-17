using AutoMapper;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatMessageDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatMessageDtos.Responses;

namespace SimpchatWeb.Services.AutoMapper.Profiles.Chats
{
    public class ChatMessageProfile : Profile
    {
        public ChatMessageProfile()
        {
            // --- GET ---
            CreateMap<Message, ChatMessageGetByIdGetResponseDto>()
                .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.Username))
                .ReverseMap();

            // --- RESPONSE ---
            CreateMap<ChatMessageGetByIdGetResponseDto, Message>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.MessageId))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.ChatId));

            CreateMap<Message, ChatMessageGetByIdGetResponseDto>()
                .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.Username));

            // --- POST ---
            CreateMap<ChatMessagePostDto, Message>();
            CreateMap<Message, ChatMessagePostDto>();

            CreateMap<ChatMessageGetByIdGetResponseDto, ChatMessagePostDto>();
            CreateMap<ChatMessagePostDto, ChatMessageGetByIdGetResponseDto>();
        }
    }
}
