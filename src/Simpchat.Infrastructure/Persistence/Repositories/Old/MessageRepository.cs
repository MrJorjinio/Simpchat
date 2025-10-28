using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Interfaces.External.FileStorage;
using Simpchat.Application.Interfaces.Repositories.Old;
using Simpchat.Application.Models.Chats.Post.Message;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Entities.Chats;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Repositories.Old
{
    public class MessageRepository : IMessageRepository
    {
        private readonly SimpchatDbContext _dbContext;
        private readonly IChatRepository _chatRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly INotificationRepository _notificationRepository;
        private const string BucketName = "chats-files";

        public MessageRepository(SimpchatDbContext dbContext, IFileStorageService fileStorageService, IChatRepository chatRepository, INotificationRepository notificationRepository)
        {
            _dbContext = dbContext;
            _fileStorageService = fileStorageService;
            _chatRepository = chatRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<Message> AddMessageAsync(PostMessageDto message, User currentUser)
        {
            string? fileUrl = null;
            if (message.FileUploadRequest?.Content != null &&
                message.FileUploadRequest.FileName != null &&
                message.FileUploadRequest.ContentType != null)
            {
                fileUrl = await _fileStorageService.UploadFileAsync(
                    BucketName,
                    message.FileUploadRequest.FileName,
                    message.FileUploadRequest.Content,
                    message.FileUploadRequest.ContentType
                );
            }

            Guid chatId;
            if (message.ChatId != null)
            {
                chatId = message.ChatId.Value;
            }
            else
            {
                var receiverId = message.ReceiverId.Value;

                var existingConversation = await _dbContext.Conversations
                    .Include(c => c.User1)
                    .Include(c => c.User2)
                    .FirstOrDefaultAsync(c =>
                        (c.UserId1 == currentUser.Id && c.UserId2 == receiverId)
                        ||
                        (c.UserId2 == currentUser.Id && c.UserId1 == receiverId)
                    );

                if (existingConversation != null)
                {
                    chatId = existingConversation.Id;
                }
                else
                {
                    var newChat = new Chat
                    {
                        Id = Guid.NewGuid(),
                        Type = ChatType.Conversation,
                        CreatedAt = DateTime.UtcNow,
                    };

                    await _dbContext.SaveChangesAsync();

                    var newConversation = new Conversation
                    {
                        Id = newChat.Id,
                        UserId1 = currentUser.Id,
                        UserId2 = receiverId
                    };

                    var newMessage = new Message
                    {
                        ChatId = newChat.Id,
                        SenderId = currentUser.Id,
                        Content = message.Content,
                        FileUrl = fileUrl,
                        ReplyId = message.ReplyId,
                        SentAt = DateTime.UtcNow
                    };

                    _dbContext.Chats.Add(newChat);
                    _dbContext.Conversations.Add(newConversation);
                    _dbContext.Messages.Add(newMessage);

                    chatId = newChat.Id;
                    message.ChatId = newConversation.Id;
                }
            }

            var messageEntity = new Message
            {
                ChatId = chatId,
                SenderId = currentUser.Id,
                Content = message.Content,
                FileUrl = fileUrl,
                ReplyId = message.ReplyId,
                SentAt = DateTime.UtcNow
            };

            if (message.ChatId != null)
            {
                _dbContext.Messages.Add(messageEntity);
                await _dbContext.SaveChangesAsync();

                var chat = await _chatRepository.GetByIdAsync(chatId);

                await _notificationRepository.AddAsync(messageEntity, chat, currentUser);
                return messageEntity;
            }
            else
            {
                return null;
            }
        }

        public async Task UpdateAsync(Message message)
        {
            _dbContext.Update(message);
            await _dbContext.SaveChangesAsync();
        }
    }
}
