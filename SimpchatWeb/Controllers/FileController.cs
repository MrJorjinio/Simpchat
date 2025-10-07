using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpchatWeb.Services.Interfaces.Minio;

namespace SimpchatWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;

        // Dependency Injection orqali IFileStorageService ni oladi
        public FileController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")] // Fayl yuklash uchun shart
        public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] string bucketName = "my-test-bucket")
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Fayl tanlanmagan yoki bo'sh.");
            }

            // Fayl nomini noyob qilish uchun Guid va original kengaytmadan foydalanamiz
            var fileExtension = Path.GetExtension(file.FileName);
            var objectName = $"{Guid.NewGuid()}{fileExtension}"; // Minio'da saqlanadigan fayl nomi

            using (var stream = file.OpenReadStream()) // Fayl streamini ochish
            {
                var fileUrl = await _fileStorageService.UploadFileAsync(bucketName, objectName, stream, file.ContentType);
                return Ok(new { Message = "Fayl muvaffaqiyatli yuklandi.", FileUrl = fileUrl });
            }
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile([FromQuery] string bucketName, [FromQuery] string objectName)
        {
            if (string.IsNullOrEmpty(bucketName) || string.IsNullOrEmpty(objectName))
            {
                return BadRequest("Bucket nomi va obyekt nomi talab qilinadi.");
            }

            try
            {
                var stream = await _fileStorageService.DownloadFileAsync(bucketName, objectName);
                // Content-Type ni aniqlashga harakat qilish yoki universal qiymat berish
                var contentType = "application/octet-stream"; // Fayl turi noma'lum bo'lsa
                                                              // Agar siz fayl turini saqlagan bo'lsangiz, uni bazadan olib foydalansangiz yaxshi bo'ladi
                return File(stream, contentType, objectName); // Faylni brauzerga jo'natish
            }
            catch (Minio.Exceptions.MinioException e) when (e.Message.Contains("Object does not exist"))
            {
                return NotFound("Fayl topilmadi.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Faylni yuklab olishda kutilmagan xatolik yuz berdi.");
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFile([FromQuery] string bucketName, [FromQuery] string objectName)
        {
            if (string.IsNullOrEmpty(bucketName) || string.IsNullOrEmpty(objectName))
            {
                return BadRequest("Bucket nomi va obyekt nomi talab qilinadi.");
            }

            try
            {
                bool removed = await _fileStorageService.RemoveFileAsync(bucketName, objectName);
                if (removed)
                {
                    return Ok("Fayl muvaffaqiyatli o'chirildi.");
                }
                return NotFound("Fayl topilmadi yoki o'chirishda muammo yuz berdi.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Faylni o'chirishda kutilmagan xatolik yuz berdi.");
            }
        }
    }
}
