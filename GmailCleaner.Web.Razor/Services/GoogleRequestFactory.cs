using GmailCleaner.Models.Settings;
using System.Net.Http.Headers;

namespace GmailCleaner.Services
{
    public interface IGoogleRequestFactory
    {
        HttpRequestMessage CreateGetEmailIdsRequest(string accessToken, string filter, string userId);
        HttpRequestMessage CreateGetEmailRequest(string accessToken, string emailId, string userId);
        public HttpRequestMessage CreateDeleteEmailRequest(string accessToken, string emailId, string userId);
        public HttpRequestMessage CreateRefreshTokenRequest(string refreshToken);
    }
    public class GoogleRequestFactory : IGoogleRequestFactory
    {

        private GoogleApiSettings _settings;
        private IConfiguration _config;

        public GoogleRequestFactory(IConfiguration config, GoogleApiSettings settings)
        {
            _settings = settings;
            _config = config;
        }
        public HttpRequestMessage CreateGetEmailIdsRequest(string accessToken, string filter, string userId)
        {
            string route = _settings.MessagesRoute + filter;
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, string.Format(route, userId));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return requestMessage;
        }
        public HttpRequestMessage CreateGetEmailRequest(string accessToken, string emailId, string userId)
        {
            string route = _settings.MessagesRoute + $"/{emailId}";
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, string.Format(route, userId));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return requestMessage;
        }
        public HttpRequestMessage CreateDeleteEmailRequest(string accessToken, string emailId, string userId)
        {
            string route = _settings.TrashRoute;
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, string.Format(route, userId, emailId));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return requestMessage;
        }
        public HttpRequestMessage CreateRefreshTokenRequest(string refreshToken)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, string.Empty);
            requestMessage.Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "client_id", _config["gmail-cleaner-client-id"] ?? string.Empty },
                { "client_secret", _config["gmail-cleaner-client-secret"] ?? string.Empty },
                { "refresh_token", refreshToken },
                { "grant_type", "refresh_token" }
            });
            return requestMessage;
        }
    }
}
