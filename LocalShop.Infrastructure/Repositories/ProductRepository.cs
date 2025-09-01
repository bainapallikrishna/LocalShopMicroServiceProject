using LocalShop.Domain.Interfaces;
using LocalShop.Domain.Models;
using LocalShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShop.Infrastructure.Repositories
{
    public class ProductRepository:IProductRepository
    {
        private readonly LocalShopDbContext _context;

        public ProductRepository(LocalShopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync() => await _context.Products.ToListAsync();

        public async Task<Product?> GetByIdAsync(int id) => await _context.Products.FindAsync(id);

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
    }
}
