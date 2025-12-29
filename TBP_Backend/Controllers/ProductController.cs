using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Price,
                    p.CategoryId,

                    Images = p.ProductImg.Select(img => new
                    {
                        img.ImageId,
                        img.ImageUrl
                    }),

                    Variants = p.ProductVariant.Select(v => new
                    {
                        v.VariantId,
                        v.Price
                    })
                })
                .ToListAsync();

            return Ok(products);
        }

        // GET: api/product/category/3
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var products = await _context.Product
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Price,

                    Images = p.ProductImg.Select(i => i.ImageUrl)
                })
                .ToListAsync();

            return Ok(products);
        }


        // GET: api/product/search?keyword=abc
        [HttpGet("search")]
        public async Task<IActionResult> Search(string keyword)
        {
            var products = await _context.Product
                .Where(p => p.ProductName.Contains(keyword))
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Price,

                    Images = p.ProductImg.Select(i => i.ImageUrl)
                })
                .ToListAsync();

            return Ok(products);
        }

    }
}
