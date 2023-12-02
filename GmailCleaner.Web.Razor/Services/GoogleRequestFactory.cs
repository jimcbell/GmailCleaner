using GmailCleaner.Models.Settings;
using System.Net.Http.Headers;

namespace GmailCleaner.Services
{
    public interface IGoogleRequestFactory
    {
        HttpRequestMessage CreateGetEmailIdsRequest(string userId, string accessToken);
        HttpRequestMessage CreateGetEmailRequest(string userId, string emailId);
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
    }
}
