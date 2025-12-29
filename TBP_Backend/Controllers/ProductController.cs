using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TBP_Backend.Data;

namespace TBP_Backend.Api
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/product
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _context.Product
                .Include(p => p.ProductImg)
                .Include(p => p.ProductVariant)
                .ToListAsync();

            return Ok(products);
        }

        // GET: api/product/category/3
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var products = await _context.Product
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.ProductImg)
                .Include(p => p.ProductVariant)
                .ToListAsync();

            return Ok(products);
        }

        // GET: api/product/search?keyword=abc
        [HttpGet("search")]
        public async Task<IActionResult> Search(string keyword)
        {
            var products = await _context.Product
                .Where(p => p.ProductName.Contains(keyword))
                .Include(p => p.ProductImg)
                .ToListAsync();

            return Ok(products);
        }
    }
}
