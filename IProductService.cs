 
using LocalShop.Model;  
using LocalShop.Repository; // Ensure the LocalShop.Repository project/assembly is referenced in your solution  
using System;  
using System.Collections.Generic;  
using System.Linq;  
using System.Text;  
using System.Threading.Tasks;  

namespace LocalShop.Service  
{  
    public interface IProductService  
    {  
        Task<ProductDto> GetProductAsync(int id);  
    }  

    public class ProductService : IProductService  
    {  
        private readonly IProductRepository _repo; // Ensure IProductRepository is defined in LocalShop.Repository  
        public ProductService(IProductRepository repo) => _repo = repo;  

        public async Task<ProductDto> GetProductAsync(int id)  
        {  
            var product = await _repo.GetByIdAsync(id);  
            return new ProductDto { Id = product.Id, Name = product.Name };  
        }  
    }  
}  
