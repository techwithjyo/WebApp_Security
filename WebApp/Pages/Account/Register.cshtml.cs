using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        public RegisterModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
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