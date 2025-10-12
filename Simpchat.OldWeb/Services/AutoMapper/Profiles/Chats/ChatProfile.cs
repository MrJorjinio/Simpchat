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
            CreateMap<Chat, ChatConversationGetByIdGetResponse>()
                .ReverseMap();
            CreateMap<ChatParticipant, UserJoinChatResponseDto>();
            
            CreateMap<Chat, GetMyChatGetResponseDto>()
                .ReverseMap();
            CreateMap<Chat, ChatConversationGetByIdGetResponse>()
                .ReverseMap();
            CreateMap<Chat, ChatGetByIdGetResponseDto>()
                .ReverseMap();

        }
    }
}
