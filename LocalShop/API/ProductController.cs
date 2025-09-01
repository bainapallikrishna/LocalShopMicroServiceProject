using LocalShop.Domain.Models;
using LocalShop.Services.DTOs;
using LocalShop.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LocalShop.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _service;
        private readonly ILogger<ProductController> _logger;


        public ProductController(ProductService service, ILogger<ProductController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("GetProduct")]
        
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpPost("CreateProduct")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody]ProductDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _service.AddProductAsync(dto);
                return CreatedAtAction(nameof(GetAll), new { id = dto.Id }, dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating product.");
                return StatusCode(500, "Internal server error");
            }

        }
    }
}
