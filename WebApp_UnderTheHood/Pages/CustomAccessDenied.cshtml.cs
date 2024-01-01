using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp_UnderTheHood.Pages
{
    public class CustomAccessDeniedModel : PageModel
    {
        [AllowAnonymous]
        public void OnGet()
        {
        }
    }
}
