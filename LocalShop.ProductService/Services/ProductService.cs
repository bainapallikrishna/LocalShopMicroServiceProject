using LocalShop.ProductService.Data;
using LocalShop.ProductService.Models;
using LocalShop.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace LocalShop.ProductService.Services
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(CreateProductRequest request);
        Task<ProductDto?> UpdateAsync(int id, UpdateProductRequest request);
        Task<bool> DeleteAsync(int id);
        Task<List<ProductDto>> GetByCategoryAsync(string category);
        Task<List<ProductDto>> SearchAsync(string searchTerm);
    }

    public class ProductService : IProductService
    {
        private readonly ProductDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ProductDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                return products.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products");
                throw;
            }
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

                return product != null ? MapToDto(product) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product by ID: {Id}", id);
                throw;
            }
        }

        public async Task<ProductDto> CreateAsync(CreateProductRequest request)
        {
            try
            {
                var product = new Product
                {
                    Name = request.Name,
                    Description = request.Description,
                    Image = request.Image,
                    Price = request.Price,
                    Category = string.Empty, // You can add category to CreateProductRequest if needed
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true,
                    StockQuantity = 0
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return MapToDto(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product: {Name}", request.Name);
                throw;
            }
        }

        public async Task<ProductDto?> UpdateAsync(int id, UpdateProductRequest request)
        {
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

                if (product == null)
                    return null;

                if (!string.IsNullOrEmpty(request.Name))
                    product.Name = request.Name;

                if (!string.IsNullOrEmpty(request.Description))
                    product.Description = request.Description;

                if (!string.IsNullOrEmpty(request.Image))
                    product.Image = request.Image;

                if (request.Price.HasValue)
                    product.Price = request.Price.Value;

                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return MapToDto(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product: {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

                if (product == null)
                    return false;

                product.IsActive = false;
                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product: {Id}", id);
                throw;
            }
        }

        public async Task<List<ProductDto>> GetByCategoryAsync(string category)
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.IsActive && p.Category.ToLower() == category.ToLower())
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                return products.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products by category: {Category}", category);
                throw;
            }
        }

        public async Task<List<ProductDto>> SearchAsync(string searchTerm)
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.IsActive && 
                               (p.Name.ToLower().Contains(searchTerm.ToLower()) ||
                                p.Description.ToLower().Contains(searchTerm.ToLower())))
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                return products.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products: {SearchTerm}", searchTerm);
                throw;
            }
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Image = product.Image,
                Price = product.Price
            };
        }
    }
}
