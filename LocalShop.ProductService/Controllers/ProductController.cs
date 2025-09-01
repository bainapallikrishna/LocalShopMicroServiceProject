using LocalShop.ProductService.Services;
using LocalShop.Shared.Infrastructure.Middleware;
using LocalShop.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalShop.ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetAll()
        {
            try
            {
                var products = await _productService.GetAllAsync();
                return Ok(ApiResponse<List<ProductDto>>.SuccessResult(products, "Products retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all products");
                return StatusCode(500, ApiResponse<List<ProductDto>>.ErrorResult("Internal server error"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound(ApiResponse<ProductDto>.ErrorResult("Product not found"));
                }

                return Ok(ApiResponse<ProductDto>.SuccessResult(product, "Product retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting product: {Id}", id);
                return StatusCode(500, ApiResponse<ProductDto>.ErrorResult("Internal server error"));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Create([FromBody] CreateProductRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<ProductDto>.ErrorResult("Invalid request data",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                var product = await _productService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, 
                    ApiResponse<ProductDto>.SuccessResult(product, "Product created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating product");
                return StatusCode(500, ApiResponse<ProductDto>.ErrorResult("Internal server error"));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Update(int id, [FromBody] UpdateProductRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<ProductDto>.ErrorResult("Invalid request data",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                var product = await _productService.UpdateAsync(id, request);
                if (product == null)
                {
                    return NotFound(ApiResponse<ProductDto>.ErrorResult("Product not found"));
                }

                return Ok(ApiResponse<ProductDto>.SuccessResult(product, "Product updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product: {Id}", id);
                return StatusCode(500, ApiResponse<ProductDto>.ErrorResult("Internal server error"));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse>> Delete(int id)
        {
            try
            {
                var success = await _productService.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(ApiResponse.ErrorResult("Product not found"));
                }

                return Ok(ApiResponse.SuccessResult("Product deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product: {Id}", id);
                return StatusCode(500, ApiResponse.ErrorResult("Internal server error"));
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetByCategory(string category)
        {
            try
            {
                var products = await _productService.GetByCategoryAsync(category);
                return Ok(ApiResponse<List<ProductDto>>.SuccessResult(products, $"Products in category '{category}' retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting products by category: {Category}", category);
                return StatusCode(500, ApiResponse<List<ProductDto>>.ErrorResult("Internal server error"));
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<List<ProductDto>>>> Search([FromQuery] string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return BadRequest(ApiResponse<List<ProductDto>>.ErrorResult("Search term is required"));
                }

                var products = await _productService.SearchAsync(q);
                return Ok(ApiResponse<List<ProductDto>>.SuccessResult(products, $"Search results for '{q}' retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching products: {SearchTerm}", q);
                return StatusCode(500, ApiResponse<List<ProductDto>>.ErrorResult("Internal server error"));
            }
        }
    }
}
