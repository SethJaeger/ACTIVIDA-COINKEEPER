using CoinKeeper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace CoinKeeper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private IConfiguration _config;

        public LoginController(AppDbContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
        }

        private async Task<User> AuthenticateUserAsync(User user)
        {
            if (_context.Users == null)
            {
                return null;
            }

            User? _user = null;

            try
            {
                using (var dbContext = _context)
                {
                    var user_ = await dbContext.Users
                        .Where(m => m.Username == user.Username && m.Password == user.Password)
                        .ToListAsync();
                    if (user_.Count > 0)
                    {
                        return user_[0];
                    }
                    else
                    {
                        return _user;
                    }
                }
            }
            catch (Exception ex)
            {
                return _user;
            }

        }

        private string GenerateToken(int userId, int roleId)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()), // User ID
                new Claim("RoleId", roleId.ToString()), // Role ID (custom claim)
            };


            var token = new JwtSecurityToken(
                _config["JWT:Issuer"],
                _config["JWT:Audience"],
                claims, // Pass the claims list here
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LoginAsync(User user)
        {
            IActionResult response = Unauthorized();

            try
            {
                using (var dbContext = _context)
                {
                    var user_ = await dbContext.Users
                        .Where(m => m.Username == user.Username)
                        .ToListAsync();
                    if (user_.Count == 0)
                    {
                        response = Ok(new { success = false, message = StatusCode(200, $"Usuario no existe") });
                    }
                    else
                    {
                        user_ = await dbContext.Users
                            .Where(m => m.Username == user.Username && m.Password == user.Password)
                            .ToListAsync();
                        if (user_.Count > 0)
                        {
                            if ((bool)user_[0].IsActive)
                            {
                                user_[0].Password = "********";
                                var token = GenerateToken(user_[0].Id, user_[0].RoleId);
                                response = Ok(new { user = user_[0], token = token });
                            }
                            else
                            {
                                response = Ok(new { success = false, message = StatusCode(200, $"Usuario inactivo") });
                            }

                        }
                        else
                        {
                            response = Ok(new { success = false, message = StatusCode(200, $"Usuario o contraseña no corresponde") });
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                response = Ok(new { success = false, message = StatusCode(500, $"An error occurred: {ex.Message}") });
            }

            return response;
        }
    }
}
