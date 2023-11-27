using CoinKeeper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoinKeeper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration? _config;

        public RolesController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rol>>> GetRoles()
        {

            return await _context.Roles.ToListAsync();
        }
    }
}
