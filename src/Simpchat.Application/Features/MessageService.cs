using Simpchat.Application.Interfaces.File;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResult;
using Simpchat.Application.Models.Chats;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Enums;

namespace Simpchat.Application.Features
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IChatRepository _chatRepo;
        private readonly IFileStorageService _fileStorageService;
        private readonly IConversationRepository _conversationRepo;
        private const string BucketName = "Messages-Files";

        public MessageService(
            IMessageRepository repo,
            IUserRepository userRepo, 
            IFileStorageService fileStorageService, 
            IChatRepository chatRepo, 
            IConversationRepository conversationRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
            _fileStorageService = fileStorageService;
            _chatRepo = chatRepo;
            _conversationRepo = conversationRepo;
        }

        public async Task<ApiResult<Guid>> SendMessageAsync(PostMessageDto postMessageDto)
        {
            string? fileUrl = null;
            if (postMessageDto.FileUploadRequest?.Content != null &&
                postMessageDto.FileUploadRequest.FileName != null &&
                postMessageDto.FileUploadRequest.ContentType != null)
            {
                fileUrl = await _fileStorageService.UploadFileAsync(
                    BucketName,
                    postMessageDto.FileUploadRequest.FileName,
                    postMessageDto.FileUploadRequest.Content,
                    postMessageDto.FileUploadRequest.ContentType
                );
            }

            Guid chatId;
            if (postMessageDto.ChatId != null)
            {
                chatId = (Guid)postMessageDto.ChatId;
            }
            else
            {

                var conversationBetweenId = await _conversationRepo.GetConversationBetweenAsync(postMessageDto.SenderId, (Guid)postMessageDto.ReceiverId);
                if (conversationBetweenId != null)
                {
                    chatId = (Guid)conversationBetweenId;
                }
                else
                {
                    var newChat = new Chat
                    {
                        Id = Guid.NewGuid(),
                        Type = ChatType.Conversation,
                    };

                    await _chatRepo.CreateAsync(newChat);

                    var newConversation = new Conversation
                    {
                        Id = newChat.Id,
                        UserId1 = postMessageDto.SenderId,
                        UserId2 = (Guid)postMessageDto.ReceiverId
                    };

                    await _conversationRepo.CreateAsync(newConversation);
                    chatId = newChat.Id;
                }
            }

            var message = new Message
            {
                ChatId = chatId,
                SenderId = postMessageDto.SenderId,
                Content = postMessageDto.Content,
                FileUrl = fileUrl,
                ReplyId = postMessageDto.ReplyId,
                SentAt = DateTime.UtcNow
            };

            await _repo.CreateAsync(message);

            return ApiResult<Guid>.SuccessResult(message.Id);
        }
    }
}
