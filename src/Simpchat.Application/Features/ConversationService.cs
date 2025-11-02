using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Chats.Get.UserChat;
using Simpchat.Domain.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Features
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly INewUserRepository _userRepo;
        private readonly IMessageRepository _messageRepo;

        public ConversationService(
            IConversationRepository conversationRepo,
            INotificationRepository notificationRepo,
            INewUserRepository userRepo,
            IMessageRepository messageRepo)
        {
            _conversationRepo = conversationRepo;
            _notificationRepo = notificationRepo;
            _userRepo = userRepo;
            _messageRepo = messageRepo;
        }

        public async Task<ApiResult> DeleteAsync(Guid conversationId)
        {
            var conversation = await _conversationRepo.GetByIdAsync(conversationId);

            if (conversation is null)
            {
                return ApiResult.FailureResult($"Conversation with ID[{conversationId}] not found");
            }

            await _conversationRepo.DeleteAsync(conversation);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult<List<UserChatResponseDto>>> GetUserConversationsAsync(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult<List<UserChatResponseDto>>.FailureResult($"User with ID[{userId}] not found");
            }

            var conversations = await _conversationRepo.GetUserConversationsAsync(userId);

            var modeledConversations = new List<UserChatResponseDto>();

            foreach (var conversation in conversations)
            {
                var notificationsCount = await _notificationRepo.GetUserChatNotificationsCountAsync(userId, conversation.Id);
                var lastMessage = await _messageRepo.GetLastMessageAsync(conversation.Id);
                var lastUserSendedMessage = await _messageRepo.GetUserLastSendedMessageAsync(userId, conversation.Id);

                var modeledConversation = new UserChatResponseDto
                {
                    AvatarUrl = conversation.UserId1 == userId ? conversation.User2.AvatarUrl : conversation.User1.AvatarUrl,
                    Name = conversation.UserId1 == userId ? conversation.User2.Username : conversation.User1.Username,
                    Id = conversation.Id,
                    Type = ChatType.Conversation,
                    LastMessage = new LastMessageResponseDto
                    {
                        Content = lastMessage.Content,
                        FileUrl = lastMessage.FileUrl,
                        SenderUsername = lastMessage.Sender.Username,
                        SentAt = lastMessage.SentAt
                    },
                    NotificationsCount = notificationsCount,
                    UserLastMessage = lastUserSendedMessage.SentAt
                };

                modeledConversations.Add(modeledConversation);
            }

            modeledConversations.OrderByDescending(mc => (DateTimeOffset?)mc.LastMessage.SentAt ?? DateTimeOffset.MinValue);

            return ApiResult<List<UserChatResponseDto>>.SuccessResult(modeledConversations);
        }
    }
}
