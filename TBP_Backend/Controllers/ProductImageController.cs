using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TBP_Backend.Models;

namespace TBP_Backend.Controllers.Api
{
    [ApiController]
    [Route("api/admin/products/{productId:int}/images")]
    [Authorize(Roles = "Admin")]
    public class ProductImageController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductImageController(AppDbContext context)
        {
            _context = context;
        }

        // ======================================
        // GET: api/admin/products/{productId}/images
        // Lấy danh sách ảnh của sản phẩm
        // ======================================
        [HttpGet]
        public async Task<IActionResult> GetImages(int productId)
        {
            if (!await _context.Product.AnyAsync(p => p.ProductId == productId))
                return NotFound(new { message = "Sản phẩm không tồn tại" });

            var images = await _context.ProductImg
                .Where(i => i.ProductId == productId)
                .Select(i => new
                {
                    i.ImageId,
                    i.ImageUrl
                })
                .ToListAsync();

            return Ok(images);
        }

        // ======================================
        // POST: api/admin/products/{productId}/images
        // Thêm ảnh cho sản phẩm
        // ======================================
        [HttpPost]
        public async Task<IActionResult> AddImage(
            int productId,
            [FromBody] ProductImageCreateDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _context.Product.AnyAsync(p => p.ProductId == productId))
                return NotFound(new { message = "Sản phẩm không tồn tại" });

            var image = new ProductImg
            {
                ProductId = productId,
                ImageUrl = model.ImageUrl
            };

            _context.ProductImg.Add(image);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Thêm ảnh thành công",
                image
            });
        }

        // ======================================
        // PUT: api/admin/products/{productId}/images/{imageId}
        // Cập nhật ảnh
        // ======================================
        [HttpPut("{imageId:int}")]
        public async Task<IActionResult> UpdateImage(
            int productId,
            int imageId,
            [FromBody] ProductImageUpdateDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var image = await _context.ProductImg
                .FirstOrDefaultAsync(i =>
                    i.ImageId == imageId &&
                    i.ProductId == productId);

            if (image == null)
                return NotFound(new { message = "Ảnh không tồn tại" });

            image.ImageUrl = model.ImageUrl;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cập nhật ảnh thành công",
                image
            });
        }

        // ======================================
        // DELETE: api/admin/products/{productId}/images/{imageId}
        // Xóa ảnh
        // ======================================
        [HttpDelete("{imageId:int}")]
        public async Task<IActionResult> DeleteImage(int productId, int imageId)
        {
            var image = await _context.ProductImg
                .FirstOrDefaultAsync(i =>
                    i.ImageId == imageId &&
                    i.ProductId == productId);

            if (image == null)
                return NotFound(new { message = "Ảnh không tồn tại" });

            _context.ProductImg.Remove(image);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa ảnh thành công" });
        }
    }

    // ======================================
    // DTOs
    // ======================================
    public class ProductImageCreateDto
    {
        [Required]
        [MaxLength(255)]
        public string ImageUrl { get; set; } = null!;
    }

    public class ProductImageUpdateDto
    {
        [Required]
        [MaxLength(255)]
        public string ImageUrl { get; set; } = null!;
    }
}
