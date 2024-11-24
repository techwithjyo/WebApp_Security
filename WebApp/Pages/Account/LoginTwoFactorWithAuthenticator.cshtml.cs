using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using System.ComponentModel.DataAnnotations;
using WebApp.Data.Account;

namespace WebApp.Pages.Account
{
    public class LoginTwoFactorWithAuthenticatorModel : PageModel
    {
        private readonly SignInManager<User> signInManager;

        [BindProperty]
        public AuthenticatorMFAViewModel AuthenticatorMFA { get; set; }
        public LoginTwoFactorWithAuthenticatorModel(SignInManager<User> signInManager)
        {
            this.AuthenticatorMFA = new AuthenticatorMFAViewModel();
            this.signInManager = signInManager;
        }
        public void OnGet(bool rememberMe)
        {
            this.AuthenticatorMFA.SecurityCode = string.Empty;
            this.AuthenticatorMFA.RememberMe = rememberMe;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) return Page();
            var result = await signInManager.TwoFactorAuthenticatorSignInAsync(this.AuthenticatorMFA.SecurityCode, 
                this.AuthenticatorMFA.RememberMe, false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Authenticator2FA", "Account is locked out");
                }
                else
                {
                    ModelState.AddModelError("Authenticator2FA", "Invalid Login Attempt");
                }

                return Page();
            }
        }
    }
    public class AuthenticatorMFAViewModel
    {
        public bool RememberMe { get; set; }
        [Required]
        [Display(Name = "Security Code")]
        public string SecurityCode { get; set; } = string.Empty;
        public Byte[]? QRCodeBytes { get; set; }
    }
}
