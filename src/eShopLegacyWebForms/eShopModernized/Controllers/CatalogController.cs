using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using eShopModernized.Models;
using eShopModernized.Services;

namespace eShopModernized.Controllers
{
    [Authorize]
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalogService;
        private readonly ILogger<CatalogController> _logger;
        private readonly IWebHostEnvironment _environment;

        public CatalogController(
            ICatalogService catalogService,
            ILogger<CatalogController> logger,
            IWebHostEnvironment environment)
        {
            _catalogService = catalogService;
            _logger = logger;
            _environment = environment;
        }

        // GET: Catalog/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catalogItem = await _catalogService.FindCatalogItemAsync(id.Value);
            if (catalogItem == null)
            {
                return NotFound();
            }

            return View(catalogItem);
        }

        // GET: Catalog/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View();
        }

        // POST: Catalog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,PictureFileName,CatalogTypeId,CatalogBrandId,AvailableStock,RestockThreshold,MaxStockThreshold,OnReorder")] CatalogItem catalogItem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _catalogService.CreateCatalogItemAsync(catalogItem);
                    _logger.LogInformation("Catalog item created successfully: {ItemName}", catalogItem.Name);
                    return RedirectToAction(nameof(Details), new { id = catalogItem.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating catalog item: {ItemName}", catalogItem.Name);
                    ModelState.AddModelError("", "Unable to save changes. Try again later.");
                }
            }

            await PopulateDropdownsAsync(catalogItem.CatalogTypeId, catalogItem.CatalogBrandId);
            return View(catalogItem);
        }

        // GET: Catalog/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catalogItem = await _catalogService.FindCatalogItemAsync(id.Value);
            if (catalogItem == null)
            {
                return NotFound();
            }

            await PopulateDropdownsAsync(catalogItem.CatalogTypeId, catalogItem.CatalogBrandId);
            return View(catalogItem);
        }

        // POST: Catalog/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,PictureFileName,CatalogTypeId,CatalogBrandId,AvailableStock,RestockThreshold,MaxStockThreshold,OnReorder")] CatalogItem catalogItem)
        {
            if (id != catalogItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _catalogService.UpdateCatalogItemAsync(catalogItem);
                    _logger.LogInformation("Catalog item updated successfully: {ItemName}", catalogItem.Name);
                    return RedirectToAction(nameof(Details), new { id = catalogItem.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating catalog item: {ItemName}", catalogItem.Name);
                    ModelState.AddModelError("", "Unable to save changes. Try again later.");
                }
            }

            await PopulateDropdownsAsync(catalogItem.CatalogTypeId, catalogItem.CatalogBrandId);
            return View(catalogItem);
        }

        // GET: Catalog/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catalogItem = await _catalogService.FindCatalogItemAsync(id.Value);
            if (catalogItem == null)
            {
                return NotFound();
            }

            return View(catalogItem);
        }

        // POST: Catalog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var catalogItem = await _catalogService.FindCatalogItemAsync(id);
            if (catalogItem != null)
            {
                await _catalogService.RemoveCatalogItemAsync(catalogItem);
                _logger.LogInformation("Catalog item deleted successfully: {ItemName}", catalogItem.Name);
            }

            return RedirectToAction("Index", "Home");
        }

        private async Task PopulateDropdownsAsync(int? selectedTypeId = null, int? selectedBrandId = null)
        {
            var types = await _catalogService.GetCatalogTypesAsync();
            var brands = await _catalogService.GetCatalogBrandsAsync();

            ViewBag.CatalogTypeId = new SelectList(types, "Id", "Type", selectedTypeId);
            ViewBag.CatalogBrandId = new SelectList(brands, "Id", "Brand", selectedBrandId);
        }
    }
}
