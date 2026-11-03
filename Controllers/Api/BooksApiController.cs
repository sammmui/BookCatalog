using BookCatalog.Data;
using BookCatalog.Dtos;
using BookCatalog.Models;
using BookCatalog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.Controllers.Api;

[ApiController]
[Route("api/books")]
public class BooksApiController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ApiCache _cache;

    public BooksApiController(AppDbContext context, ApiCache cache)
    {
        _context = context;
        _cache = cache;
    }

    private static BookDto ToDto(Book b) => new()
    {
        Id = b.Id, Title = b.Title, Year = b.Year, Pages = b.Pages,
        Isbn = b.Isbn, Description = b.Description, CoverPath = b.CoverPath,
        AuthorId = b.AuthorId, AuthorName = b.Author?.Name
    };

    [HttpGet]
    public async Task<ActionResult<PagedResult<BookDto>>> GetAll(int skip = 0, int limit = 10)
    {
        if (skip < 0 || limit < 1 || limit > 100)
            return BadRequest("skip must be >= 0 and limit must be 1..100");

        var key = $"books:list:{skip}:{limit}";
        if (_cache.TryGet(key, out PagedResult<BookDto>? cached) && cached != null)
        {
            Response.Headers["X-Cache"] = "HIT";
            return Ok(cached);
        }

        var total = await _context.Books.CountAsync();
        var books = await _context.Books.Include(b => b.Author)
            .OrderBy(b => b.Id).Skip(skip).Take(limit).ToListAsync();

        string? next = skip + limit < total
            ? $"{Request.Scheme}://{Request.Host}/api/books?skip={skip + limit}&limit={limit}"
            : null;

        var result = new PagedResult<BookDto>
        {
            Items = books.Select(ToDto).ToList(),
            Total = total, Skip = skip, Limit = limit, NextLink = next
        };

        _cache.Set(key, result);
        Response.Headers["X-Cache"] = "MISS";
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> Get(int id)
    {
        var key = $"books:{id}";
        if (_cache.TryGet(key, out BookDto? cached) && cached != null)
        {
            Response.Headers["X-Cache"] = "HIT";
            return Ok(cached);
        }

        var book = await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id);
        if (book == null) return NotFound();

        var dto = ToDto(book);
        _cache.Set(key, dto);
        Response.Headers["X-Cache"] = "MISS";
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> Create(BookDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title)) return BadRequest("Title is required");
        if (!await _context.Authors.AnyAsync(a => a.Id == dto.AuthorId))
            return BadRequest("Author not found");

        var book = new Book
        {
            Title = dto.Title, Year = dto.Year, Pages = dto.Pages, Isbn = dto.Isbn,
            Description = dto.Description, CoverPath = dto.CoverPath, AuthorId = dto.AuthorId
        };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        _cache.Clear();
        dto.Id = book.Id;
        return CreatedAtAction(nameof(Get), new { id = book.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, BookDto dto)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();
        if (string.IsNullOrWhiteSpace(dto.Title)) return BadRequest("Title is required");
        if (!await _context.Authors.AnyAsync(a => a.Id == dto.AuthorId))
            return BadRequest("Author not found");

        book.Title = dto.Title; book.Year = dto.Year; book.Pages = dto.Pages;
        book.Isbn = dto.Isbn; book.Description = dto.Description;
        book.CoverPath = dto.CoverPath; book.AuthorId = dto.AuthorId;
        await _context.SaveChangesAsync();
        _cache.Clear();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        _cache.Clear();
        return NoContent();
    }
}