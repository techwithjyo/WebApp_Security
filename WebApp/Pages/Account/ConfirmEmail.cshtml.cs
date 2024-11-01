using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Data.Account;

namespace WebApp.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        public ConfirmEmailModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        [BindProperty]
        public string Message { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user != null)
            {
                var result = _userManager.ConfirmEmailAsync(user, token);
                if(result.Result.Succeeded)
                {
                    Message = "Email Address is successfully confirmed, you can now login.";
                    return Page();
                }
            }
            this.Message = "Failed to validate email.";
            return Page();
        }
    }
}
