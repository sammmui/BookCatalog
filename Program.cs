using Microsoft.EntityFrameworkCore;
using BookCatalog.Services;
using BookCatalog.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ApiCache>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapGet("/sitemap.xml", async (HttpContext ctx, AppDbContext db) =>
{
    var baseUrl = $"{ctx.Request.Scheme}://{ctx.Request.Host}";
    var ids = await db.Books.Select(b => b.Id).ToListAsync();
    var sb = new System.Text.StringBuilder();
    sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
    sb.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
    sb.Append($"<url><loc>{baseUrl}/</loc></url>");
    sb.Append($"<url><loc>{baseUrl}/Books</loc></url>");
    foreach (var id in ids)
        sb.Append($"<url><loc>{baseUrl}/Books/Details/{id}</loc></url>");
    sb.Append("</urlset>");
    return Results.Content(sb.ToString(), "application/xml");
});

app.Run();
