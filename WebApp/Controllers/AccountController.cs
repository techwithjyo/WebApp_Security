using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.Data.Account;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly SignInManager<User> signInManager;

        public AccountController(SignInManager<User> signInManager)
        {
            this.signInManager = signInManager;
        }
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var loginInfo = await signInManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                var emailClaim = loginInfo.Principal.FindFirst(ClaimTypes.Email);
                var userName = loginInfo.Principal.FindFirst(ClaimTypes.Name);

                if (emailClaim != null && userName != null)
                {
                    var user = new User { Email = emailClaim.Value,
                        UserName = userName.Value
                    };
                    await signInManager.SignInAsync(user, false);
                }
                return RedirectToPage("/Index");
            }
        }
    }
}
