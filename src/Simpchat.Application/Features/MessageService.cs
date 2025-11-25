using FluentValidation;
using Simpchat.Application.Errors;
using Simpchat.Application.Extentions;
using Simpchat.Application.Interfaces.File;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResult;

using Simpchat.Application.Models.Files;
using Simpchat.Application.Models.Messages;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Enums;
using Simpchat.Shared.Models;

namespace Simpchat.Application.Features
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IChatRepository _chatRepo;
        private readonly IFileStorageService _fileStorageService;
        private readonly IConversationRepository _conversationRepo;
        private readonly IValidator<PostMessageDto> _postMessageValidator;
        private readonly IValidator<UpdateMessageDto> _updateMessageValidator;
        private readonly IChatBanRepository _chatBanRepository;
        private const string BucketName = "Messages-Files";

        public MessageService(
            IMessageRepository repo,
            IUserRepository userRepo,
            IFileStorageService fileStorageService,
            IChatRepository chatRepo,
            IConversationRepository conversationRepo,
            IValidator<PostMessageDto> postMessageValidator,
            IValidator<UpdateMessageDto> updateMessageValidator,
            IChatBanRepository chatBanRepository)
        {
            _repo = repo;
            _userRepo = userRepo;
            _fileStorageService = fileStorageService;
            _chatRepo = chatRepo;
            _conversationRepo = conversationRepo;
            _postMessageValidator = postMessageValidator;
            _updateMessageValidator = updateMessageValidator;
            _chatBanRepository = chatBanRepository;
        }

        public async Task<Result<Guid>> SendMessageAsync(PostMessageDto postMessageDto, UploadFileRequest? uploadFileRequest)
        {
            var validationResult = await _postMessageValidator.ValidateAsync(postMessageDto);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                  .GroupBy(e => e.PropertyName)
                  .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return Result.Failure<Guid>(ApplicationErrors.Validation.Failed, errors);
            }

            var sender = await _userRepo.GetByIdAsync(postMessageDto.SenderId);
            if (sender is null)
            {
                return Result.Failure<Guid>(ApplicationErrors.User.IdNotFound);
            }

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
                    return Result.Failure<Guid>(ApplicationErrors.Message.IdNotFound);
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
                        Type = ChatTypes.Conversation,
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

            var chat = await _chatRepo.GetByIdAsync(chatId);
            if (chat is null)
            {
                return Result.Failure<Guid>(ApplicationErrors.Chat.IdNotFound);
            }

            var isBanned = await _chatBanRepository.IsUserBannedAsync(chatId, postMessageDto.SenderId);
            if (isBanned)
            {
                return Result.Failure<Guid>(ApplicationErrors.ChatPermission.Denied);
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

            return message.Id;
        }

        public async Task<Result> UpdateAsync(Guid messageId, UpdateMessageDto updateMessageDto, UploadFileRequest? uploadFileRequest, Guid userId)
        {
            var validationResult = await _updateMessageValidator.ValidateAsync(updateMessageDto);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                  .GroupBy(e => e.PropertyName)
                  .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return Result.Failure<Guid>(ApplicationErrors.Validation.Failed, errors);
            }

            var message = await _repo.GetByIdAsync(messageId);

            if (message is null)
            {
                return Result.Failure(ApplicationErrors.Message.IdNotFound);
            }

            if (!message.IsMessageSender(userId))
            {
                return Result.Failure(ApplicationErrors.ChatPermission.Denied);
            }

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
                    return Result.Failure(ApplicationErrors.Message.IdNotFound);
                }
            }

            if (fileUrl is null)
            {
                fileUrl = message.FileUrl;
            }

            message.FileUrl = fileUrl;
            message.Content = updateMessageDto.Content;
            message.ReplyId = updateMessageDto.ReplyId;

            await _repo.UpdateAsync(message);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(Guid messageId, Guid userId)
        {
            var message = await _repo.GetByIdAsync(messageId);

            if (message is null)
            {
                return Result.Failure(ApplicationErrors.Message.IdNotFound);
            }

            if (!message.IsMessageSender(userId))
            {
                return Result.Failure(ApplicationErrors.ChatPermission.Denied);
            }

            await _repo.DeleteAsync(message);

            return Result.Success();
        }
    }
}
