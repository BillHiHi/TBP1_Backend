using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TBP_Backend.Models;

namespace TBP_Backend.Api
{
    [ApiController]
    [Route("api/admin/account")]
    [Authorize(Roles = "Admin")]
    public class AccountManagementController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountManagementController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // =============================================
        // GET: api/admin/account/users
        // =============================================
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users = _userManager.Users.Select(u => new
            {
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                u.PhoneNumber
            });

            return Ok(users);
        }

        // =============================================
        // GET: api/admin/account/user/{id}
        // =============================================
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name);

            return Ok(new
            {
                user = new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.PhoneNumber
                },
                roles,
                allRoles
            });
        }

        // =============================================
        // PUT: api/admin/account/user/{id}/role
        // =============================================
        [HttpPut("user/{id}/role")]
        public async Task<IActionResult> UpdateRole(string id, UpdateRoleRequest request)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (!await _roleManager.RoleExistsAsync(request.Role))
                return BadRequest("Role không tồn tại");

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, request.Role);

            return Ok(new { message = "Cập nhật role thành công" });
        }

        // =============================================
        // DELETE: api/admin/account/user/{id}
        // =============================================
        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            await _userManager.DeleteAsync(user);
            return Ok(new { message = "Đã xóa user" });
        }
    }

    public class UpdateRoleRequest
    {
        public string Role { get; set; } = null!;
    }
}
