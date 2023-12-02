
using Microsoft.AspNetCore.Authentication;

namespace GmailCleaner.Services
{
    public interface IUserContextService
    {
        public string GetToken(HttpRequest request);

    }
    public class UserContextService : IUserContextService
    {
        public string GetToken(HttpRequest request)
        {
            //userId = request.HttpContext.User.Claims.Where(c => c.Type == "sub").Select(c => c.Value).FirstOrDefault() ?? string.Empty;
            string accessToken = request.HttpContext.GetTokenAsync("access_token").Result ?? string.Empty;
            string refreshToken = request.HttpContext.GetTokenAsync("refresh_token").Result ?? string.Empty;
            string idToken = request.HttpContext.GetTokenAsync("id_token").Result ?? string.Empty;
            return accessToken;
        }
    }
}
