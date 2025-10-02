using AutoMapper;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Responses;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Temps;

namespace SimpchatWeb.Services.AutoMapper.Profiles
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<ChatMessageResponseDto, ChatMessagePostDto>();
            CreateMap<ChatMessagePostDto, ChatMessageResponseDto>();        
            CreateMap<ChatMessageTempDto, ChatMessageResponseDto>();
            CreateMap<ChatMessageResponseDto, ChatMessageTempDto>();
            CreateMap<ChatMessageResponseDto, Message>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.MessageId));
            CreateMap<ChatMessagePostDto, Message>();
            CreateMap<ChatMessageTempDto, Message>();
            CreateMap<ChatMessageConversationPostDto, Message>();
            CreateMap<Message, ChatMessageResponseDto>()
                .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.Id));
            CreateMap<Message, ChatMessagePostDto>();
            CreateMap<Message, ChatMessageTempDto>();
            CreateMap<Message, ChatMessageConversationPostDto>();
        }
    }
}
