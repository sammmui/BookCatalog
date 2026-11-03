using BookCatalog.Data;
using BookCatalog.Dtos;
using BookCatalog.Models;
using BookCatalog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.Controllers.Api;

[ApiController]
[Route("api/authors")]
public class AuthorsApiController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ApiCache _cache;

    public AuthorsApiController(AppDbContext context, ApiCache cache)
    {
        _context = context;
        _cache = cache;
    }

    private static AuthorDto ToDto(Author a) => new()
    {
        Id = a.Id, Name = a.Name, BirthPlace = a.BirthPlace,
        BirthYear = a.BirthYear, Latitude = a.Latitude, Longitude = a.Longitude
    };

    [HttpGet]
    public async Task<ActionResult<PagedResult<AuthorDto>>> GetAll(int skip = 0, int limit = 10)
    {
        if (skip < 0 || limit < 1 || limit > 100)
            return BadRequest("skip must be >= 0 and limit must be 1..100");

        var total = await _context.Authors.CountAsync();
        var items = await _context.Authors.OrderBy(a => a.Id).Skip(skip).Take(limit).ToListAsync();
        string? next = skip + limit < total
            ? $"{Request.Scheme}://{Request.Host}/api/authors?skip={skip + limit}&limit={limit}" : null;

        return Ok(new PagedResult<AuthorDto>
        {
            Items = items.Select(ToDto).ToList(),
            Total = total, Skip = skip, Limit = limit, NextLink = next
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorDto>> Get(int id)
    {
        var a = await _context.Authors.FindAsync(id);
        return a == null ? NotFound() : Ok(ToDto(a));
    }

    [HttpPost]
    public async Task<ActionResult<AuthorDto>> Create(AuthorDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Name is required");
        var a = new Author
        {
            Name = dto.Name, BirthPlace = dto.BirthPlace, BirthYear = dto.BirthYear,
            Latitude = dto.Latitude, Longitude = dto.Longitude
        };
        _context.Authors.Add(a);
        await _context.SaveChangesAsync();
        _cache.Clear();
        dto.Id = a.Id;
        return CreatedAtAction(nameof(Get), new { id = a.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, AuthorDto dto)
    {
        var a = await _context.Authors.FindAsync(id);
        if (a == null) return NotFound();
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Name is required");
        a.Name = dto.Name; a.BirthPlace = dto.BirthPlace; a.BirthYear = dto.BirthYear;
        a.Latitude = dto.Latitude; a.Longitude = dto.Longitude;
        await _context.SaveChangesAsync();
        _cache.Clear();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var a = await _context.Authors.FindAsync(id);
        if (a == null) return NotFound();
        if (await _context.Books.AnyAsync(b => b.AuthorId == id))
            return Conflict("Author has books and cannot be deleted");
        _context.Authors.Remove(a);
        await _context.SaveChangesAsync();
        _cache.Clear();
        return NoContent();
    }
}