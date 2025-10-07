using AutoMapper;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Responses;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatMessageDtos.Responses;

namespace SimpchatWeb.Services.AutoMapper.Profiles.Chats
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            // --- RESPONSE ---
            CreateMap<ChatConversationGetByIdGetResponse, Chat>()
                .ReverseMap();
            CreateMap<Chat, ChatConversationGetByIdGetResponse>()
                .ReverseMap();
            CreateMap<ChatParticipant, UserJoinChatResponseDto>();
            CreateMap<Message, ChatMessageGetByIdGetResponseDto>()
                .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.Username))
                .ReverseMap();
            CreateMap<Chat, GetMyChatGetResponseDto>()
                .ReverseMap();
            CreateMap<Chat, ChatConversationGetByIdGetResponse>()
                .ReverseMap();
            CreateMap<Chat, ChatGetByIdGetResponseDto>()
                .ReverseMap();

        }
    }
}
