
using GmailCleaner.Models.ExternalModels;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace GmailCleaner.Services
{
    public interface IUserContextService
    {
        public GmailToken GetToken(HttpRequest request);
        public string GetUserId(HttpRequest request);
        public GmailUserModel GetUser(HttpRequest request);

    }
    public class UserContextService : IUserContextService
    {
        public GmailToken GetToken(HttpRequest request)
        {
            //userId = request.HttpContext.User.Claims.Where(c => c.Type == "sub").Select(c => c.Value).FirstOrDefault() ?? string.Empty;

            // *** Super helpful to see all authentication properties below
            //var result = request.HttpContext.AuthenticateAsync();
            //var properties = result.Result.Properties;
            string accessToken = request.HttpContext.GetTokenAsync("access_token").Result ?? string.Empty;
            string refreshToken = request.HttpContext.GetTokenAsync("refresh_token").Result ?? string.Empty;
            DateTime expitesAt = DateTime.Parse(request.HttpContext.GetTokenAsync("expires_at").Result ?? string.Empty);
            GmailToken gmailToken = new(accessToken, refreshToken, expitesAt);
            return gmailToken;
        }
        public string GetUserId(HttpRequest request)
        {
            string userId = request.HttpContext.User.Claims.Where(c => c.Type == "id").Select(c => c.Value).FirstOrDefault() ?? string.Empty;
            return userId;
        }
        public GmailUserModel GetUser(HttpRequest request)
        {
            string name = request.HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).FirstOrDefault() ?? string.Empty;
            string userId = request.HttpContext.User.Claims.Where(c => c.Type == "id").Select(c => c.Value).FirstOrDefault() ?? string.Empty;
            GmailUserModel user = new(userId, name, "");
            return user;
        }
    }
}
