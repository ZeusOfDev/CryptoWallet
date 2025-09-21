using Microsoft.AspNetCore.Mvc;
using CryptoWalletApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Azure.Core;
using CryptoWalletApp.Utility;
using CryptoWalletApp.Repositories;
using Microsoft.AspNetCore.Identity;
using CryptoWalletApp.Services;

namespace CryptoWalletApp.Controllers
{
    [ApiController]
    [Route("/api/")]
    public class AuthenticationController : ControllerBase
    {
        private readonly CryptoWalletContext dbcontext;
        private readonly IConfiguration configuration;
        private readonly IRefreshTokenRepository dprepo;
        public AuthenticationController(CryptoWalletContext _dbcontext, IConfiguration configuration, IRefreshTokenRepository dprepo) 
        {
            this.dbcontext = _dbcontext;
            this.configuration = configuration;
            this.dprepo = dprepo;
        }
        [HttpGet("login")]
        public IActionResult Login([FromHeader(Name = "Refresh-Token")] string refreshToken)
        {
            if (HttpContext.User.Identity?.IsAuthenticated == true)
            {
                return Ok(new
                {
                    redirect = "/home"
                });
            }
            if (!dprepo.IsExistRefreshToken(refreshToken))
            {
                return Unauthorized(new { redirect = "/login",exist = refreshToken });
            }
            var user = dprepo.GetUserRoleDtoFromRefreshToken(refreshToken);
            if (user == null)
                return NotFound("dont have user with this refresh token");
            return Ok(new 
            { 
                accessToken = TokenGenerator.GenerateJwtToken(configuration, user) 
            });
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (HttpContext.User.Identity?.IsAuthenticated == true)
            {
                return Ok(new
                {
                    redirect = "/home"
                });
            }
            var userLogin = dbcontext.Users
                .Where(u => u.UserName == loginModel.Username && 
                        u.PasswordHash == Hashing.UserPasswordHashsing(loginModel.Password)).FirstOrDefault();

            if (userLogin == null)
            {
                return Unauthorized("Wrong username or password");
            }
            var userRoleDto = new UserRoleDto
            {
                UserId = userLogin.UserId,
                UserName = loginModel.Username,
                RoleIDs = dbcontext.UserRoles.Where(ur => ur.UserId == userLogin.UserId).Select(ur => ur.RoleId).ToList()
            };

            var refreshToken = dbcontext.RefreshTokens.FirstOrDefault<RefreshToken>(rt => rt.UserId == userLogin.UserId);
            if (refreshToken?.Token == default)
            {
                var newrefreshToken = TokenGenerator.GenerateRefreshToken();
                dbcontext.RefreshTokens.Add(new RefreshToken { Token = newrefreshToken, IsRevoked = false, UserId = userLogin.UserId });
                await dbcontext.SaveChangesAsync();
                return Ok(new
                {
                    refreshToken = newrefreshToken,
                    accessToken = TokenGenerator.GenerateJwtToken(configuration, userRoleDto),
                });
            }
            if (refreshToken.IsRevoked == true)
            {
                return Unauthorized("you has been banned");
            }
            return Ok(new
            {
                refreshToken = refreshToken.Token,
                accessToken = TokenGenerator.GenerateJwtToken(configuration, userRoleDto),
            });
        }
        [HttpPost("signup")]
        [AllowAnonymous] 
        public async Task<IActionResult> SignUp([FromBody]SignupModel input)
        {
            var user = dbcontext.Users.Where(u => u.UserName == input.Username || u.Email == input.Email).FirstOrDefault();
            if (user != null)
            {
                return BadRequest("username or email has already exist");
            }
            if (input.Password != input.ConfirmPassword)
            {

            }
            var basicRole = dbcontext.Roles.AsTracking().FirstOrDefault(r => r.RoleId == "default");
            
            if (basicRole == null)
                return BadRequest("Default role not found.");
            var newUser = new User
            {
                Email = input.Email,
                PasswordHash = Hashing.UserPasswordHashsing(input.Password),
                UserName = input.Username,
                UserBalance = 0.00m,
            };

            dbcontext.Users.Add(newUser);
            dbcontext.SaveChanges();

            var newUserID = dbcontext.Users.Where(u => u.UserName == input.Username).Select(u => u.UserId).FirstOrDefault();
            dbcontext.UserCryptos.AddRange(new List<UserCrypto>
            {
                new UserCrypto { UserId = newUserID, CryptoId = "btc", Ucbalance = 0.001000m},
                new UserCrypto { UserId = newUserID, CryptoId = "eth", Ucbalance = 0.01000m},
                new UserCrypto { UserId = newUserID, CryptoId = "usdt", Ucbalance = 0.00m}
            });

            dbcontext.UserRoles.Add(new UserRole
            {
               UserId = newUserID,
               RoleId = "default"
            });
            var roweffected = await dbcontext.SaveChangesAsync();

            return Ok(new
            {
                Message = "You has created account successfully",
                user = newUser.UserName,
                role = basicRole.RoleId,
                roweffected = roweffected
            });
        }
    }
}
