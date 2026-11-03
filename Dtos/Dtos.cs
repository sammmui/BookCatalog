namespace BookCatalog.Dtos;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int? Year { get; set; }
    public int Pages { get; set; }
    public string? Isbn { get; set; }
    public string? Description { get; set; }
    public string? CoverPath { get; set; }
    public int AuthorId { get; set; }
    public string? AuthorName { get; set; }
}

public class AuthorDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? BirthPlace { get; set; }
    public int? BirthYear { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class GenreDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
    public int Skip { get; set; }
    public int Limit { get; set; }
    public string? NextLink { get; set; }
}