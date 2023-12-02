using GmailCleaner.Models.Settings;
using System.Net.Http.Headers;

namespace GmailCleaner.Services
{
    public interface IGoogleRequestFactory
    {
        HttpRequestMessage CreateGetEmailIdsRequest(string userId, string accessToken);
        HttpRequestMessage CreateGetEmailRequest(string userId, string emailId);
        public HttpRequestMessage CreateRefreshTokenRequest(string refreshToken);
    }
    public class GoogleRequestFactory : IGoogleRequestFactory
    {

        private GoogleApiSettings _settings;

        public GoogleRequestFactory(GoogleApiSettings settings)
        {
            _settings = settings;
        }
        public HttpRequestMessage CreateGetEmailIdsRequest(string accessToken, string filter)
        {
            string route = _settings.MessagesRoute + filter;
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, string.Format(route, "me"));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return requestMessage;
        }
        public HttpRequestMessage CreateGetEmailRequest(string accessToken, string emailId)
        {
            string route = _settings.MessagesRoute + $"/{emailId}";
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, string.Format(route, "me"));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return requestMessage;
        }
        public HttpRequestMessage CreateRefreshTokenRequest(string refreshToken)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, string.Empty);
            requestMessage.Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "client_id", _settings.ClientId },
                { "client_secret", _settings.ClientSecret },
                { "refresh_token", refreshToken },
                { "grant_type", "refresh_token" }
            });
            return requestMessage;
        }
    }
}
