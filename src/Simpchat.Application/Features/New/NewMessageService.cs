using Simpchat.Application.Interfaces.External.FileStorage;
using Simpchat.Application.Interfaces.Repositories.New;
using Simpchat.Application.Interfaces.Repositories.Old;
using Simpchat.Application.Interfaces.Services.New;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Chats.Post.Message;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Entities.Chats;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Features.New
{
    public class NewMessageService : INewMessageService
    {
        private readonly INewMessageRepository _repo;
        private readonly INewUserRepository _userRepo;
        private readonly INewChatRepository _chatRepo;
        private readonly IFileStorageService _fileStorageService;
        private readonly INewConversationRepository _conversationRepo;
        private const string BucketName = "Messages-Files";

        public NewMessageService(
            INewMessageRepository repo,
            INewUserRepository userRepo, 
            IFileStorageService fileStorageService, 
            INewChatRepository chatRepo, 
            INewConversationRepository conversationRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
            _fileStorageService = fileStorageService;
            _chatRepo = chatRepo;
            _conversationRepo = conversationRepo;
        }

        public async Task<ApiResult> AddMessage(PostMessageDto postMessageDto)
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

            return ApiResult.SuccessResult();
        }
    }
}
