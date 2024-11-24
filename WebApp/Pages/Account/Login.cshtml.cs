using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;
using WebApp.Data.Account;

namespace WebApp.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        public LoginModel(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }
        [BindProperty]
        public CredentialViewModel Credential { get; set; } = new CredentialViewModel();
        public async Task OnGet()
        {
           
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await _signInManager.PasswordSignInAsync(Credential.Email, Credential.Password, Credential.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("/Account/LoginTwoFactorWithAuthenticator",
                        new
                        {
                            this.Credential.RememberMe
                        });
                }
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login", "Account is locked out");
                }
                else
                {
                    ModelState.AddModelError("Login", "Invalid Login Attempt");
                }

                return Page();
            }
        }
    }

    public class CredentialViewModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
