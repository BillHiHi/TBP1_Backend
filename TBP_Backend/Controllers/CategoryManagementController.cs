using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TBP_Backend.Models;

namespace TBP_Backend.Controllers.Api
{
    [ApiController]
    [Route("api/admin/categories")]
    [Authorize(Roles = "Admin")]
    public class CategoryManagementController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryManagementController(AppDbContext context)
        {
            _context = context;
        }

        // ======================================
        // GET: api/admin/categories
        // Lấy danh sách category
        // ======================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
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
        // GET: api/admin/categories/{id}
        // Lấy chi tiết category
        // ======================================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _context.Categories
                .Where(c => c.CategoryId == id)
                .Select(c => new
                {
                    c.CategoryId,
                    c.CategoryName
                })
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound(new { message = "Category không tồn tại" });

            return Ok(category);
        }

        // ======================================
        // POST: api/admin/categories
        // Thêm category
        // ======================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = new Category
            {
                CategoryName = model.Name,
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Thêm category thành công",
                category
            });
        }

        // ======================================
        // PUT: api/admin/categories/{id}
        // Cập nhật category
        // ======================================
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound(new { message = "Category không tồn tại" });

            category.CategoryName = model.Name;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cập nhật category thành công",
                category
            });
        }

        // ======================================
        // DELETE: api/admin/categories/{id}
        // Xóa category
        // ======================================
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound(new { message = "Category không tồn tại" });

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa category thành công" });
        }
    }

    // ======================================
    // DTOs
    // ======================================
    public class CategoryCreateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class CategoryUpdateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
