using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using WebApp.Services;

namespace WebApp.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _emailService;

        public RegisterModel(UserManager<IdentityUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
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
            var user = new IdentityUser
            {
                Email = RegisterModel1.Email,
                UserName = RegisterModel1.Email
            };

            var result = await this._userManager.CreateAsync(user, RegisterModel1.Password);
            if (result.Succeeded)
            {
                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail", values: new { userId = user.Id, token = confirmationToken });

                await _emailService.SendAsync("jyotirmoy.professional@gmail.com", user.Email, "Please Confirm your email", $"Please click this link to confirm your email address: {confirmationLink}");
                
                return RedirectToPage("/Account/Login");
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
        }
    }
}