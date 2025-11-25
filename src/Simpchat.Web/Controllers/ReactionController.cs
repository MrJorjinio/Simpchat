using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simpchat.Application.Extentions;
using Simpchat.Application.Interfaces.Services;

using Simpchat.Application.Models.Files;
using Simpchat.Application.Models.Reactions;

namespace Simpchat.Web.Controllers
{
    [Route("api/reactions")]
    [ApiController]
    public class ReactionController : ControllerBase
    {
        private readonly IReactionService _reactionService;

        public ReactionController(IReactionService reactionService)
        {
            _reactionService = reactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var response = await _reactionService.GetAllAsync();
            var apiResponse = response.ToApiResult();

            return apiResponse.ToActionResult();
        }

        [HttpGet("{reactionId}")]
        public async Task<IActionResult> GetByIdAsync(Guid reactionId)
        {
            var response = await _reactionService.GetByIdAsync(reactionId);
            var apiResponse = response.ToApiResult();

            return apiResponse.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] PostReactionDto postReactionDto, IFormFile? file)
        {
            UploadFileRequest? fileUploadRequest = null;

            if (file != null)
            {
                fileUploadRequest = new UploadFileRequest
                {
                    Content = file.OpenReadStream(),
                    FileName = file.FileName,
                    ContentType = file.ContentType
                };
            }

            var response = await _reactionService.CreateAsync(postReactionDto, fileUploadRequest);
            var apiResponse = response.ToApiResult();

            return apiResponse.ToActionResult();
        }

        [HttpPut("{reactionId}")]
        public async Task<IActionResult> UpdateAsync(Guid reactionId, [FromForm] UpdateReactionDto updateReactionDto, IFormFile? file)
        {
            UploadFileRequest? fileUploadRequest = null;

            if (file != null)
            {
                fileUploadRequest = new UploadFileRequest
                {
                    Content = file.OpenReadStream(),
                    FileName = file.FileName,
                    ContentType = file.ContentType
                };
            }

            var response = await _reactionService.UpdateAsync(reactionId, updateReactionDto, fileUploadRequest);
            var apiResponse = response.ToApiResult();

            return apiResponse.ToActionResult();
        }

        [HttpDelete("{reactionId}")]
        public async Task<IActionResult> DeleteAsync(Guid reactionId)
        {
            var response = await _reactionService.DeleteAsync(reactionId);
            var apiResponse = response.ToApiResult();

            return apiResponse.ToActionResult();
        }
    }
}
