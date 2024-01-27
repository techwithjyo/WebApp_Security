using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Web_API.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IConfiguration configuration;

        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        [HttpPost]
        public IActionResult Authenticate([FromBody] Credential credential)
        {
            if (credential.UserName == "admin" && credential.Password == "password")
            {
                //creating the security context
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, credential.UserName),
                    new Claim(ClaimTypes.Email, credential.UserName + "@gmail.com"),
                    new Claim("Department", "HR"),
                    new Claim("Admin", "true"),
                    new Claim("Manager", "true"),
                    new Claim("EmploymentDate", "2022-05-01")
                };

                var expiresAt = DateTime.UtcNow.AddMinutes(10);

                return Ok(new
                {
                    access_token = CreateToken(claims, expiresAt),
                    expires_at = expiresAt
                });
            }

            ModelState.AddModelError("Unauthorized", "You are not authorized to access the endpoint");
            return Unauthorized(ModelState);
        }

        private string CreateToken(IEnumerable<Claim> claims, DateTime expireAt)
        {
            var secretKey = Encoding.ASCII.GetBytes(configuration.GetValue<string>("SecretKey")??"");
            var jwt = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expireAt,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature));
                
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }

    public class Credential
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
