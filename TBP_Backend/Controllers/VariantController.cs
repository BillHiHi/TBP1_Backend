using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TBP_Backend.Models;

namespace TBP_Backend.Api
{
    [ApiController]
    [Route("api/variants")]
    public class VariantController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VariantController(AppDbContext context)
        {
            _context = context;
        }

        // =========================================
        // GET: api/variants/product/5
        // Lấy variant theo Product
        // =========================================
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(int productId)
        {
            var variants = await _context.ProductVariant
                .Where(v => v.ProductId == productId)
                .Select(v => new
                {
                    v.VariantId,
                    v.Size,
                    v.Colors,
                    v.Price,
                    v.Stock
                })
                .ToListAsync();

            return Ok(variants);
        }

        // =========================================
        // POST: api/variants
        // Thêm variant
        // =========================================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateVariantDto model)
        {
            var productExists = await _context.Product
                .AnyAsync(p => p.ProductId == model.ProductId);

            if (!productExists)
                return BadRequest("Product không tồn tại");

            var variant = new ProductVariant
            {
                ProductId = model.ProductId,
                Size = model.Size,
                Colors = model.Colors,
                Price = model.Price,
                Stock = model.Stock
            };

            _context.ProductVariant.Add(variant);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Thêm variant thành công",
                variant
            });
        }

        // =========================================
        // PUT: api/variants/{id}
        // Cập nhật variant
        // =========================================
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateVariantDto model)
        {
            var variant = await _context.ProductVariant.FindAsync(id);
            if (variant == null)
                return NotFound("Variant không tồn tại");

            variant.Size = model.Size;
            variant.Colors = model.Colors;
            variant.Price = model.Price;
            variant.Stock = model.Stock;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật thành công" });
        }

        // =========================================
        // DELETE: api/variants/{id}
        // Xóa variant
        // =========================================
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var variant = await _context.ProductVariant.FindAsync(id);
            if (variant == null)
                return NotFound("Variant không tồn tại");

            _context.ProductVariant.Remove(variant);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa variant thành công" });
        }
    }

    // =========================================
    // DTOs
    // =========================================
    public class CreateVariantDto
    {
        public int ProductId { get; set; }
        public string Size { get; set; }
        public string Colors { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }

    public class UpdateVariantDto
    {
        public string Size { get; set; }
        public string Colors { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
