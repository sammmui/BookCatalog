using Microsoft.AspNetCore.Mvc;

namespace BookCatalog.Controllers.Api;

[ApiController]
[Route("api/covers")]
public class CoversApiController : ControllerBase
{
    private static readonly string[] Allowed = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
    private const long MaxSize = 2 * 1024 * 1024;
    private readonly IWebHostEnvironment _env;

    public CoversApiController(IWebHostEnvironment env) => _env = env;

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("File is empty");
        if (file.Length > MaxSize) return BadRequest("File is too large (max 2 MB)");
        if (!file.ContentType.StartsWith("image/")) return BadRequest("Only images are allowed");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!Allowed.Contains(ext)) return BadRequest("Unsupported file type");

        var dir = Path.Combine(_env.WebRootPath, "covers");
        Directory.CreateDirectory(dir);
        var name = Guid.NewGuid() + ext;
        await using var fs = new FileStream(Path.Combine(dir, name), FileMode.Create);
        await file.CopyToAsync(fs);

        return Ok(new { path = "/covers/" + name });
    }
}