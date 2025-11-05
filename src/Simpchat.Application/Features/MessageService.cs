using Simpchat.Application.Interfaces.File;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResult;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Files;
using Simpchat.Application.Models.Messages;
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

        public async Task<ApiResult<Guid>> SendMessageAsync(PostMessageDto postMessageDto, UploadFileRequest? uploadFileRequest)
        {
            string? fileUrl = null;
            if (uploadFileRequest.Content != null &&
                uploadFileRequest.FileName != null &&
                uploadFileRequest.ContentType != null)
            {
                fileUrl = await _fileStorageService.UploadFileAsync(
                    BucketName,
                    uploadFileRequest.FileName,
                    uploadFileRequest.Content,
                    uploadFileRequest.ContentType
                );
            }

            if (postMessageDto.ReplyId is not null)
            {
                if (await _repo.GetByIdAsync((Guid)postMessageDto.ReplyId) is null)
                {
                    return ApiResult<Guid>.FailureResult($"Reply-Message ID[{postMessageDto.ReplyId} not found");
                }
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

        public async Task<ApiResult> UpdateAsync(Guid messageId, UpdateMessageDto updateMessageDto, UploadFileRequest? uploadFileRequest)
        {
            string? fileUrl = null;
            if (uploadFileRequest.Content != null &&
                uploadFileRequest.FileName != null &&
                uploadFileRequest.ContentType != null)
            {
                fileUrl = await _fileStorageService.UploadFileAsync(
                    BucketName,
                    uploadFileRequest.FileName,
                    uploadFileRequest.Content,
                    uploadFileRequest.ContentType
                );
            }

            if (updateMessageDto.ReplyId is not null)
            {
                if (await _repo.GetByIdAsync((Guid)updateMessageDto.ReplyId) is null)
                {
                    return ApiResult.FailureResult($"Reply-Message ID[{updateMessageDto.ReplyId} not found");
                }
            }

            var message = await _repo.GetByIdAsync(messageId);

            if (message is null)
            {
                return ApiResult.FailureResult($"Message with ID[{messageId}] not found");
            }

            if (fileUrl is null)
            {
                fileUrl = message.FileUrl;
            }

            message.FileUrl = fileUrl;
            message.Content = updateMessageDto.Content;
            message.ReplyId = updateMessageDto.ReplyId;

            await _repo.UpdateAsync(message);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> DeleteAsync(Guid messageId)
        {
            var message = await _repo.GetByIdAsync(messageId);

            if (message is null)
            {
                return ApiResult.FailureResult($"Message with ID[{messageId}] not found");
            }

            await _repo.DeleteAsync(message);

            return ApiResult.SuccessResult();
        }
    }
}
