using BookCatalog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.Controllers.Api;

[ApiController]
[Route("api/stats")]
public class StatsApiController : ControllerBase
{
    private readonly AppDbContext _context;
    public StatsApiController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var byYear = await _context.Books
            .Where(b => b.Year != null)
            .GroupBy(b => b.Year)
            .Select(g => new { year = g.Key, count = g.Count() })
            .OrderBy(x => x.year)
            .ToListAsync();

        var byAuthor = await _context.Books
            .GroupBy(b => b.Author.Name)
            .Select(g => new { author = g.Key, count = g.Count() })
            .OrderByDescending(x => x.count)
            .Take(8)
            .ToListAsync();

        var longest = await _context.Books
            .OrderByDescending(b => b.Pages)
            .Take(8)
            .Select(b => new { title = b.Title, pages = b.Pages })
            .ToListAsync();

        return Ok(new
        {
            totalBooks = await _context.Books.CountAsync(),
            totalAuthors = await _context.Authors.CountAsync(),
            totalPages = await _context.Books.SumAsync(b => (int?)b.Pages) ?? 0,
            byYear,
            byAuthor,
            longest
        });
    }
}