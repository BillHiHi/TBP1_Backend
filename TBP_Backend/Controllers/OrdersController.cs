using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TBP_Backend.Dto;
using TBP_Backend.Models;

namespace TBP_Backend.Api
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // ======================================
        // POST: api/orders/checkout
        // ======================================
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto model)
        {
            if (model == null || model.Items == null || !model.Items.Any())
                return BadRequest("Giỏ hàng trống");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Lấy danh sách Variant
            var variantIds = model.Items.Select(i => i.VariantId).ToList();
            var variants = await _context.ProductVariant
                .Where(v => variantIds.Contains(v.VariantId))
                .ToListAsync();

            // Tính tổng tiền
            decimal total = 0;
            foreach (var item in model.Items)
            {
                var variant = variants.FirstOrDefault(v => v.VariantId == item.VariantId);
                if (variant == null)
                    return BadRequest("Variant không tồn tại");

                if (variant.Stock < item.Quantity)
                    return BadRequest("Sản phẩm không đủ tồn kho");

                total += variant.Price * item.Quantity;
            }

            // 1️⃣ Tạo Order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = total,
                Status = "Pending"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // 👉 Có OrderId

            // 2️⃣ Tạo OrderItems
            foreach (var item in model.Items)
            {
                var variant = variants.First(v => v.VariantId == item.VariantId);

                var orderItem = new OrderItems
                {
                    OrderId = order.OrderId,
                    VariantId = variant.VariantId,
                    Quantity = item.Quantity,
                    Price = variant.Price
                };

                _context.OrderItems.Add(orderItem); // ✅ ĐÚNG
                variant.Stock -= item.Quantity;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Đặt hàng thành công",
                orderId = order.OrderId
            });
        }

        // ======================================
        // GET: api/orders/my
        // ======================================
        [HttpGet("my")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                    o.TotalAmount,
                    o.Status
                })
                .ToListAsync();

            return Ok(orders);
        }
    }
}
