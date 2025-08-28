using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UnoSquareProducts.API.Data;

namespace mx.unosquare.products.api.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UnoSquareProductsAPIContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        public TokenController(IConfiguration configuration, UnoSquareProductsAPIContext context, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginKeyPair loginData)
        {
            try
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginData.username, loginData.password,
                    isPersistent: false,
                    lockoutOnFailure: false);

                if (!result.Succeeded)
                {
                    return Unauthorized();
                }
                IdentityUser user = await _userManager.FindByEmailAsync(loginData.username);
                if (user.LockoutEnd.HasValue)
                {
                    return Unauthorized(new { error = "Blocked account" });
                }
                IList<string> roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Usuario API"))
                {
                    return Unauthorized(new { error = "Not valid user assignments" });
                }
                JwtSecurityToken token = await GenerateTokenAsync(user, roles);
                //defined        
                string serializedToken = new JwtSecurityTokenHandler().WriteToken(token);
                //serialize the token        
                return Ok(new SuccessfulLoginResult() { Token = serializedToken });
            }
            catch (Exception ex)
            {
                Log.Error("++ Error al obtener el token ++");
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                if (ex.InnerException != null)
                {
                    Log.Error("==== INNER EXCEPTION ======");
                    Log.Error(ex.InnerException.Message);
                    Log.Error(ex.InnerException.StackTrace);
                }
                return new StatusCodeResult(500);
            }
        }
        private async Task<JwtSecurityToken> GenerateTokenAsync(IdentityUser user, IList<string> roles)
        {
            var claims = new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
            };
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            // Loading the user Claims
            var expirationDays = _configuration.GetValue<int>("Jwt:TokenExpirationDays");
            var siginingKey = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Jwt:SigningKey"));
            var token = new JwtSecurityToken
            (
                issuer: _configuration.GetValue<string>("Jwt:Issuer"),
                audience: _configuration.GetValue<string>("Jwt:Audience"),
                claims: claims,
                expires: DateTime.Now.Add(TimeSpan.FromDays(expirationDays)),
                notBefore: DateTime.Now,
                //signingCredentials: new SigningCredentials(new SymmetricSecurityKey(siginingKey), SecurityAlgorithms.HmacSha256)
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(siginingKey), SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

    }
    public class TokenResponse
    {
        public string token { get; set; }
        public string validUntil { get; set; }
    }

    public class LoginKeyPair
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    public class SuccessfulLoginResult
    {
        public string Token { get; set; }
    }
}
