using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TBP_Backend.Models;

namespace TBP_Backend.Api
{
    [ApiController]
    [Route("api/cart")]
    [Authorize] 
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // =========================================
        // GET: api/cart
        // Lấy giỏ hàng của user hiện tại
        // =========================================
        [HttpGet]
        public async Task<IActionResult> GetMyCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductVariant)
                        .ThenInclude(v => v.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);


            if (cart == null)
                return Ok(new { items = new List<object>() });

            var result = cart.CartItems.Select(ci => new
            {
                ci.CartItemID,
                ci.Quantity,
                ci.ProductVariant.VariantId,
                ci.ProductVariant.Size,
                ci.ProductVariant.Colors,
                Price = ci.ProductVariant.Price,
                Product = new
                {
                    ci.ProductVariant.Product.ProductId,
                    ci.ProductVariant.Product.ProductName
                }
            });

            return Ok(result);
        }

        // =========================================
        // POST: api/cart/add
        // Thêm sản phẩm vào giỏ
        // =========================================
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(AddToCartDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. Lấy hoặc tạo cart
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.Now
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // 2. Kiểm tra item đã tồn tại chưa
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci =>
                    ci.CartID == cart.CartId &&
                    ci.VariantID == model.VariantId);

            if (cartItem != null)
            {
                cartItem.Quantity += model.Quantity;
            }
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    CartID = cart.CartId,
                    VariantID = model.VariantId,
                    Quantity = model.Quantity
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã thêm vào giỏ hàng" });
        }

        // =========================================
        // PUT: api/cart/update
        // Cập nhật số lượng
        // =========================================
        [HttpPut("update")]
        public async Task<IActionResult> UpdateQuantity(UpdateCartDto model)
        {
            var cartItem = await _context.CartItems.FindAsync(model.CartItemId);

            if (cartItem == null)
                return NotFound(new { message = "Cart item không tồn tại" });

            cartItem.Quantity = model.Quantity;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật thành công" });
        }

        // =========================================
        // DELETE: api/cart/remove/{id}
        // Xóa item khỏi giỏ
        // =========================================
        [HttpDelete("remove/{id}")]
        public async Task<IActionResult> RemoveItem(int id)
        {
            var item = await _context.CartItems.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa sản phẩm khỏi giỏ" });
        }
    }

    // =========================================
    // DTOs
    // =========================================
    public class AddToCartDto
    {
        public int VariantId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartDto
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
