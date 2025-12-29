using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TBP_Backend.Models;

namespace TBP_Backend.Controllers.Api
{
    [ApiController]
    [Route("api/admin/products")]
    [Authorize(Roles = "Admin")]
    public class ProductManagementController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductManagementController(AppDbContext context)
        {
            _context = context;
        }

        // ======================================
        // GET: api/admin/products
        // Lấy tất cả sản phẩm
        // ======================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
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
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(products);
        }

        // ======================================
        // GET: api/admin/products/{id}
        // Lấy chi tiết sản phẩm
        // ======================================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
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
                    p.CategoryId,
                    Images = _context.ProductImg
                        .Where(i => i.ProductId == p.ProductId)
                        .Select(i => i.ImageUrl)
                        .ToList(),
                    Variants = _context.ProductVariant
                        .Where(v => v.ProductId == p.ProductId)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound(new { message = "Sản phẩm không tồn tại" });

            return Ok(product);
        }

        // ======================================
        // POST: api/admin/products
        // Thêm sản phẩm
        // ======================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = new Product
            {
                ProductName = model.ProductName,
                Description = model.Description,
                Price = model.Price,
                OldPrice = model.OldPrice,
                CategoryId = model.CategoryId
            };

            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            // Thêm ảnh
            if (model.Images != null)
            {
                foreach (var img in model.Images)
                {
                    _context.ProductImg.Add(new ProductImg
                    {
                        ProductId = product.ProductId,
                        ImageUrl = img
                    });
                }
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                message = "Thêm sản phẩm thành công",
                product
            });
        }

        // ======================================
        // PUT: api/admin/products/{id}
        // Cập nhật sản phẩm
        // ======================================
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _context.Product.FindAsync(id);
            if (product == null)
                return NotFound(new { message = "Sản phẩm không tồn tại" });

            product.ProductName = model.ProductName;
            product.Description = model.Description;
            product.Price = model.Price;
            product.OldPrice = model.OldPrice;
            product.CategoryId = model.CategoryId;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cập nhật sản phẩm thành công",
                product
            });
        }

        // ======================================
        // DELETE: api/admin/products/{id}
        // Xóa sản phẩm
        // ======================================
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
                return NotFound(new { message = "Sản phẩm không tồn tại" });

            // Xóa ảnh
            var images = _context.ProductImg.Where(i => i.ProductId == id);
            _context.ProductImg.RemoveRange(images);

            // Xóa variant
            var variants = _context.ProductVariant.Where(v => v.ProductId == id);
            _context.ProductVariant.RemoveRange(variants);

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa sản phẩm thành công" });
        }
    }

    // ======================================
    // DTOs
    // ======================================
    public class ProductCreateDto
    {
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public int CategoryId { get; set; }
        public List<string>? Images { get; set; }
    }

    public class ProductUpdateDto
    {
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public int CategoryId { get; set; }
    }
}
