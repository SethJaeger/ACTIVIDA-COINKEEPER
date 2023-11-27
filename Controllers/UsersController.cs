using CoinKeeper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CoinKeeper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration? _config;

        public UsersController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {


            var users = await _context.Users
                .Select(u => new User
                {
                    Id = u.Id,
                    Firstname = u.Firstname,
                    Lastname = u.Lastname,
                    Username = u.Username,
                    Email = u.Email,
                    RoleId = u.RoleId,
                    IsActive = u.IsActive
                }).Where(m => m.IsActive == true).ToListAsync();


            return users;
            //return await _context.Users.ToListAsync();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<User>>> PostUsers(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();

            var user_ = await _context.Users
                .Where(m => m.Id == user.Id)
                .ToListAsync();
            if (user_ == null)
            {
                return NotFound();
            }

            return Ok(user_);

        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {

            var user = await _context.Users
                            .Where(m => m.Id == id)
                            .ToListAsync();
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound();
                }

                _context.Users.Remove(user);
                _context.SaveChanges();

                return Ok(new { success = true, message = StatusCode(200, $"Usuario eliminado con éxito") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = StatusCode(200, $"Usuario no pudo ser eliminado") });
            }
        }

    }
}
