 
using Microsoft.AspNetCore.Http;  
using Microsoft.AspNetCore.Mvc;  
using LocalShop.Services; // Corrected namespace to 'Services'  
namespace LocalShop.API  
{  
    [Route("api/[controller]")]  
    [ApiController]  
    public class ProductController : ControllerBase  
    {  
        private readonly IProductService _service;  
        public ProductController(IProductService service) => _service = service;  

        [HttpGet("{id}")]  
        public async Task<IActionResult> Get(int id)  
        {  
            var product = await _service.GetProductAsync(id);  
            return Ok(product);  
        }  
    }  
}  
