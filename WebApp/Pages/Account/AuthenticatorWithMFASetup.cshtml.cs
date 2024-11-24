using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Data.Account;
using System.ComponentModel.DataAnnotations;
using QRCoder;

namespace WebApp.Pages.Account
{
    [Authorize]
    public class AuthenticatorWithMFASetupModel : PageModel
    {
        private readonly UserManager<User> userManager;
        [BindProperty]
        public SetupMFAViewModel ViewModel { get; set; }
        public bool Succeeded { get; set; }

        public AuthenticatorWithMFASetupModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.ViewModel = new SetupMFAViewModel();
            this.Succeeded = false;
        }
        public async Task OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                await userManager.ResetAuthenticatorKeyAsync(user);
                var authKey = await userManager.GetAuthenticatorKeyAsync(user);

                this.ViewModel.Key = authKey ?? string.Empty;
                this.ViewModel.QRCodeBase64 = Convert.ToBase64String(GenerateQRCodeBytes("TechWithJoe", authKey, user.Email));
            }
        }
        private Byte[] GenerateQRCodeBytes(string provider, string key, string userEmail)
        {
            var qrCodeGenerator = new QRCodeGenerator();
            var qrCodeData = qrCodeGenerator.CreateQrCode($"otpauth://totp/{provider}:{userEmail}?secret={key}&issuer={provider}",
                QRCodeGenerator.ECCLevel.Q);

            var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var isValid = await userManager.VerifyTwoFactorTokenAsync(user, userManager.Options.Tokens.AuthenticatorTokenProvider,
                ViewModel.SecurityCode);
            if (!isValid)
            {
                ModelState.AddModelError("SecurityCode", "Invalid security code");
                return Page();
            }
            await userManager.SetTwoFactorEnabledAsync(user, true);
            Succeeded = true;
            return Page();
        }

        public class SetupMFAViewModel
        {
            public string? Key { get; set; }
            [Required]
            [Display(Name = "Security Code")]
            public string SecurityCode { get; set; } = string.Empty;
            public string? QRCodeBase64 { get; set; }
        }
    }
}