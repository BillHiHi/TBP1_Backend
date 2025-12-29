using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TBP_Backend.Controllers.Api
{
    [ApiController]
    [Route("api/menu")]
    public class MenuController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MenuController(AppDbContext context)
        {
            _context = context;
        }

        // ======================================
        // GET: api/menu/categories
        // Lấy danh sách category
        // ======================================
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new
                {
                    c.CategoryId,
                    c.CategoryName
                })
                .ToListAsync();

            return Ok(categories);
        }

        // ======================================
        // GET: api/menu/category/{id}
        // Lấy sản phẩm theo category
        // ======================================
        [HttpGet("category/{id:int}")]
        public async Task<IActionResult> GetProductsByCategory(int id)
        {
            var products = await _context.Product
                .Where(p => p.CategoryId == id)
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Description,
                    p.Price,
                    p.OldPrice,
                    Images = _context.ProductImg
                        .Where(i => i.ProductId == p.ProductId)
                        .Select(i => i.ImageUrl)
                        .ToList()
                })
                .ToListAsync();

            if (!products.Any())
                return NotFound(new { message = "Không có sản phẩm trong danh mục này" });

            return Ok(products);
        }

        // ======================================
        // GET: api/menu/products
        // Lấy tất cả sản phẩm
        // ======================================
        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.Product
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Description,
                    p.Price,
                    p.OldPrice,
                    CategoryName = _context.Categories
                        .Where(c => c.CategoryId == p.CategoryId)
                        .Select(c => c.CategoryName)
                        .FirstOrDefault(),
                    Images = _context.ProductImg
                        .Where(i => i.ProductId == p.ProductId)
                        .Select(i => i.ImageUrl)
                        .ToList()
                })
                .ToListAsync();

            return Ok(products);
        }

        // ======================================
        // GET: api/menu/product/{id}
        // Lấy chi tiết sản phẩm
        // ======================================
        [HttpGet("product/{id:int}")]
        public async Task<IActionResult> GetProductDetail(int id)
        {
            var product = await _context.Product
                .Where(p => p.ProductId == id)
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Description,
                    p.Price,
                    p.OldPrice,
                    CategoryName = _context.Categories
                        .Where(c => c.CategoryId == p.CategoryId)
                        .Select(c => c.CategoryName)
                        .FirstOrDefault(),
                    Images = _context.ProductImg
                        .Where(i => i.ProductId == p.ProductId)
                        .Select(i => i.ImageUrl)
                        .ToList(),
                    Variants = _context.ProductVariant
                        .Where(v => v.ProductId == p.ProductId)
                        .Select(v => new
                        {
                            v.VariantId,
                            v.Size,
                            v.Colors,
                            v.Price,
                            v.Stock
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound(new { message = "Không tìm thấy sản phẩm" });

            return Ok(product);
        }
    }
}
