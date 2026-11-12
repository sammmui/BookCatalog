using BookCatalog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.Controllers.Api;

[ApiController]
[Route("api/authors/search")]
public class AuthorSearchApiController : ControllerBase
{
    private const int PageSize = 20;
    private readonly AppDbContext _context;
    public AuthorSearchApiController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> Search(string? term, int page = 1)
    {
        term = term?.Trim();
        if (string.IsNullOrEmpty(term) || term.Length < 3)
            return BadRequest("term must be at least 3 characters");
        if (page < 1) page = 1;

        var items = await _context.Authors
            .Where(a => a.Name.Contains(term))
            .OrderBy(a => a.Name)
            .Skip((page - 1) * PageSize)
            .Take(PageSize + 1)
            .Select(a => new { id = a.Id, text = a.Name })
            .ToListAsync();

        return Ok(new
        {
            results = items.Take(PageSize),
            more = items.Count > PageSize
        });
    }
}