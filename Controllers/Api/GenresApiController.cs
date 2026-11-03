using BookCatalog.Data;
using BookCatalog.Dtos;
using BookCatalog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.Controllers.Api;

[ApiController]
[Route("api/genres")]
public class GenresApiController : ControllerBase
{
    private readonly AppDbContext _context;
    public GenresApiController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<PagedResult<GenreDto>>> GetAll(int skip = 0, int limit = 10)
    {
        if (skip < 0 || limit < 1 || limit > 100)
            return BadRequest("skip must be >= 0 and limit must be 1..100");

        var total = await _context.Genres.CountAsync();
        var items = await _context.Genres.OrderBy(g => g.Id).Skip(skip).Take(limit)
            .Select(g => new GenreDto { Id = g.Id, Name = g.Name }).ToListAsync();
        string? next = skip + limit < total
            ? $"{Request.Scheme}://{Request.Host}/api/genres?skip={skip + limit}&limit={limit}" : null;

        return Ok(new PagedResult<GenreDto> { Items = items, Total = total, Skip = skip, Limit = limit, NextLink = next });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GenreDto>> Get(int id)
    {
        var g = await _context.Genres.FindAsync(id);
        return g == null ? NotFound() : Ok(new GenreDto { Id = g.Id, Name = g.Name });
    }

    [HttpPost]
    public async Task<ActionResult<GenreDto>> Create(GenreDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Name is required");
        if (await _context.Genres.AnyAsync(g => g.Name == dto.Name)) return Conflict("Genre already exists");
        var g = new Genre { Name = dto.Name };
        _context.Genres.Add(g);
        await _context.SaveChangesAsync();
        dto.Id = g.Id;
        return CreatedAtAction(nameof(Get), new { id = g.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, GenreDto dto)
    {
        var g = await _context.Genres.FindAsync(id);
        if (g == null) return NotFound();
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Name is required");
        g.Name = dto.Name;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var g = await _context.Genres.FindAsync(id);
        if (g == null) return NotFound();
        _context.Genres.Remove(g);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}