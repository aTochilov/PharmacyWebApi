using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyApi.Data;
using PharmacyApi.Data.Entities;
using PharmacyApi.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PharmacyApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly PharmacyDbContext context;
        private readonly AuthService authService;

        public AuthController(PharmacyDbContext context, AuthService authService)
        {
            this.context = context;
            this.authService = authService;
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Register(UserDto request)
        {
            try
            {
                authService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                var user = new User()
                {
                    Username = request.Username,
                    Password = passwordHash,
                    PasswordSalt = passwordSalt,
                    isAdmin = request.isAdmin
                };
                await context.AddAsync(user);
                await context.SaveChangesAsync();
                return Ok("Saved");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ex.Message);
            }
            
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var user = context.Users.FirstOrDefault(u => u.Username == request.Username);
            if (user is null) 
                return NotFound("User not found");
            if (!authService.VerifyPasswordHash(request.Password, user.Password, user.PasswordSalt))
                return BadRequest("Wrong password");
            return Ok(authService.CreateToken(user));
        }

        [HttpPost("delete")]
        public async Task<ActionResult> Delete(UserDto request)
        {
            var user = context.Users.FirstOrDefault(u => u.Username == request.Username);
            if (user is null)
                return NotFound("User not found");
            if (!authService.VerifyPasswordHash(request.Password, user.Password, user.PasswordSalt))
                return BadRequest("Wrong password");
            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return Ok("Removed");
        }
    }
}
