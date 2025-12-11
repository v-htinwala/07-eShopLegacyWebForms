using Microsoft.EntityFrameworkCore;
using eShopModernized.Data;
using eShopModernized.Models;
using eShopModernized.ViewModels;

namespace eShopModernized.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly CatalogDbContext _db;
        private readonly ILogger<CatalogService> _logger;

        public CatalogService(CatalogDbContext db, ILogger<CatalogService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<PaginatedItemsViewModel<CatalogItem>> GetCatalogItemsPaginatedAsync(int pageSize, int pageIndex)
        {
            var totalItems = await _db.CatalogItems.LongCountAsync();

            var itemsOnPage = await _db.CatalogItems
                .Include(c => c.CatalogBrand)
                .Include(c => c.CatalogType)
                .OrderBy(c => c.Id)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} catalog items (page {Page}, size {PageSize})", 
                itemsOnPage.Count, pageIndex, pageSize);

            return new PaginatedItemsViewModel<CatalogItem>(
                pageIndex, pageSize, totalItems, itemsOnPage);
        }

        public async Task<CatalogItem?> FindCatalogItemAsync(int id)
        {
            return await _db.CatalogItems
                .Include(c => c.CatalogBrand)
                .Include(c => c.CatalogType)
                .FirstOrDefaultAsync(ci => ci.Id == id);
        }

        public async Task<IEnumerable<CatalogType>> GetCatalogTypesAsync()
        {
            return await _db.CatalogTypes.ToListAsync();
        }

        public async Task<IEnumerable<CatalogBrand>> GetCatalogBrandsAsync()
        {
            return await _db.CatalogBrands.ToListAsync();
        }

        public async Task CreateCatalogItemAsync(CatalogItem catalogItem)
        {
            // Generate next ID if not set
            if (catalogItem.Id == 0)
            {
                var maxId = await _db.CatalogItems.AnyAsync() 
                    ? await _db.CatalogItems.MaxAsync(c => c.Id) 
                    : 0;
                catalogItem.Id = maxId + 1;
            }

            _db.CatalogItems.Add(catalogItem);
            await _db.SaveChangesAsync();
            
            _logger.LogInformation("Created catalog item {ItemId}: {ItemName}", 
                catalogItem.Id, catalogItem.Name);
        }

        public async Task UpdateCatalogItemAsync(CatalogItem catalogItem)
        {
            _db.Entry(catalogItem).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            
            _logger.LogInformation("Updated catalog item {ItemId}: {ItemName}", 
                catalogItem.Id, catalogItem.Name);
        }

        public async Task RemoveCatalogItemAsync(CatalogItem catalogItem)
        {
            _db.CatalogItems.Remove(catalogItem);
            await _db.SaveChangesAsync();
            
            _logger.LogInformation("Removed catalog item {ItemId}: {ItemName}", 
                catalogItem.Id, catalogItem.Name);
        }
    }
}
