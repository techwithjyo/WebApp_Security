using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Security.Claims;
using WebApp.Data.Account;
using WebApp.Services;
using WebApp.Settings;

namespace WebApp.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IOptions<SmtpSettings> smtpSettings;

        public RegisterModel(UserManager<User> userManager, IEmailService emailService, IOptions<SmtpSettings> smtpSettings)
        {
            _userManager = userManager;
            _emailService = emailService;
            this.smtpSettings = smtpSettings;
        }
        [BindProperty]
        public RegisterViewModel RegisterModel1 { get; set; } = new RegisterViewModel();
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            //Validate Email Address (optional)

            //Create a User 
            var user = new User
            {
                Email = RegisterModel1.Email,
                UserName = RegisterModel1.Email,
            };

            var claimDepartment = new Claim("Department", RegisterModel1.Department);
            var claimPosition = new Claim("Position", RegisterModel1.Position);
            var claimCompanyCode = new Claim("CompanyCode", RegisterModel1.CompanyCode);

            var result = await this._userManager.CreateAsync(user, RegisterModel1.Password);

            if (result.Succeeded)
            {
                await this._userManager.AddClaimAsync(user, claimDepartment);
                await this._userManager.AddClaimAsync(user, claimPosition);
                await this._userManager.AddClaimAsync(user, claimCompanyCode);
                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                if (smtpSettings.Value.EmailSentFromDemoServer.ToString().ToLower() == "true")
                {
                    return Redirect(Url.PageLink(pageName: "/Account/ConfirmEmail", values: new { userId = user.Id, token = confirmationToken }) ?? "");
                }
                else
                {
                    var confirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail", values: new { userId = user.Id, token = confirmationToken });

                    await _emailService.SendAsync("jyotirmoy.professional@gmail.com", user.Email, "Please Confirm your email", $"Please click this link to confirm your email address: {confirmationLink}");

                    return RedirectToPage("/Account/Login");
                }
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
        }
        public class RegisterViewModel
        {
            [Required]
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string Email { get; set; } = string.Empty;
            [Required]
            [DataType(dataType: DataType.Password)]
            public string Password { get; set; } = string.Empty;
            [Required]
            public string Department { get; set; } = string.Empty;
            [Required]
            public string Position { get; set; } = string.Empty;
            [Required]
            public string CompanyCode { get; set; } = string.Empty;
        }
    }
}