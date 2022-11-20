using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PharmacyApi.Data;
using PharmacyApi.Data.DTO;
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
        private readonly ILogger<AuthController> logger;

        public AuthController(PharmacyDbContext context, AuthService authService, ILogger<AuthController> log)
        {
            this.context = context;
            this.authService = authService;
            logger = log;
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Register(UserRegister userRequest)
        {
            try
            {
                authService.CreatePasswordHash(userRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);
                var user = new User()
                {
                    Username = userRequest.Username,
                    Password = passwordHash,
                    PasswordSalt = passwordSalt,
                    isAdmin = userRequest.isAdmin
                };
                await context.AddAsync(user);
                await context.SaveChangesAsync();
                return Ok("Saved");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Failed to register user");
            }

        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto userRequest)
        {
            try { 
                var user = context.Users.FirstOrDefault(u => u.Username == userRequest.Username);
                if (user is null)
                    if(context.Users.Any())
                        return NotFound("User not found");
                    else{
                        await Register(new UserRegister{
                            Username = userRequest.Username,
                            Password = userRequest.Password,
                            isAdmin = true});
                        user = context.Users.FirstOrDefault(u => u.Username == userRequest.Username);
                    }
                if (!authService.VerifyPasswordHash(userRequest.Password, user.Password, user.PasswordSalt))
                    return BadRequest("Wrong password");
                return Ok(authService.CreateToken(user));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Failed to log in");
    }
}

        [HttpPost("delete")]
        [Authorize]
        public async Task<ActionResult> Delete(UserDto userRequest)
        {
            try
            {
                var user = context.Users.FirstOrDefault(u => u.Username == userRequest.Username);
            if (user is null)
                return NotFound("User not found");
            if (!authService.VerifyPasswordHash(userRequest.Password, user.Password, user.PasswordSalt))
                return BadRequest("Wrong password");

                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return Ok("Removed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Failed to delete user");
            }
        }

        [HttpPost("password")]
        [Authorize]
        public async Task<ActionResult<string>> ChangePassword([FromBody] UserChangePwd userRequest)
        {
            try
            {
                var currentUser = context.Users.FirstOrDefault(u => u.Username == userRequest.Username);
            if (currentUser is null)
                return NotFound("User not found");
            if (!authService.VerifyPasswordHash(userRequest.Password, currentUser.Password, currentUser.PasswordSalt))
                return BadRequest("Wrong password");

            authService.CreatePasswordHash(userRequest.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
            currentUser.Password = passwordHash;
            currentUser.PasswordSalt = passwordSalt;

                context.Entry(currentUser).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok("Saved");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Failed to change password");
            }
        }
    }
}
