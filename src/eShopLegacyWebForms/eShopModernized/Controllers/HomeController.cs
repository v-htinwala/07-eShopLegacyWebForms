using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eShopModernized.Models;
using eShopModernized.Services;

namespace eShopModernized.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICatalogService _catalogService;
    private const int PageSize = 10;

    public HomeController(ILogger<HomeController> _logger, ICatalogService catalogService)
    {
        this._logger = _logger;
        _catalogService = catalogService;
    }

    public async Task<IActionResult> Index(int? page)
    {
        var pageIndex = page ?? 0;
        var viewModel = await _catalogService.GetCatalogItemsPaginatedAsync(PageSize, pageIndex);
        return View(viewModel);
    }

    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
