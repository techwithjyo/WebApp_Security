using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using WebApp_UnderTheHood.DTO;
using WebApp_UnderTheHood.Pages.Account;

namespace WebApp_UnderTheHood.Pages
{
    [Authorize(Policy = "HRManagerOnly")]
    public class HRManagerModel : PageModel
    {
        private readonly IHttpClientFactory httpClientFactory;

        [BindProperty]
        public List<WeatheForecastDTO> weatherForecastItems { get; set; } = new List<WeatheForecastDTO>();

        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public async Task OnGetAsync()
        {
            //get token from session
            JwtToken token = new JwtToken();

            var strTokenObj = HttpContext.Session.GetString("access_token");
            if(string.IsNullOrEmpty(strTokenObj))
            {
                token = await Authenticate();
            }
            else
            {
                token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj)??new JwtToken();
            }

            if (token == null || 
                string.IsNullOrWhiteSpace(token.AccessToken) ||
                token.ExpiresAt <= DateTime.UtcNow)
            {
               token = await Authenticate();
            }
            var httpClient = httpClientFactory.CreateClient("OurWebAPI");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken ?? string.Empty);
            weatherForecastItems = await httpClient.GetFromJsonAsync<List<WeatheForecastDTO>>("WeatherForecast") ?? new List<WeatheForecastDTO>();
        }

        private async Task<JwtToken> Authenticate()
        {
            var httpClient = httpClientFactory.CreateClient("OurWebAPI");

            var response = await httpClient.PostAsJsonAsync("auth", new Credential { UserName = "admin", Password = "password" });
            response.EnsureSuccessStatusCode();
            string strJwt = await response.Content.ReadAsStringAsync();

            HttpContext.Session.SetString("access_token", strJwt);

            return JsonConvert.DeserializeObject<JwtToken>(strJwt)??new JwtToken();
        }
    }
}
