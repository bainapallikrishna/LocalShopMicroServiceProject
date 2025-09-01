using AutoMapper;
using LocalShop.Domain.Interfaces;
using LocalShop.Domain.Models;
using LocalShop.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShop.Services.Services
{
    public class ProductService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        public ProductService(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;

        }

       

        public async Task<Product?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task AddProductAsync(Product product) => await _repository.AddAsync(product);
        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task AddProductAsync(ProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            await _repository.AddAsync(product);
        }

    }
}
