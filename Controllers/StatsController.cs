using Microsoft.AspNetCore.Mvc;

namespace BookCatalog.Controllers;

public class StatsController : Controller
{
    public IActionResult Index() => View();
}