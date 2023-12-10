using GmailCleaner.Models.Data;
using GmailCleaner.Models.ExternalModels;
using GmailCleaner.Models.Settings;
using GmailCleaner.Services;
using System.Runtime;
using System.Text;

namespace GmailCleaner.Managers
{
    public interface IEmailManager
    {
        public void LoadAccessToken(string accessToken);
        public Task<Email> GetEmail(string emailId);
        public Task<List<Email>> GetEmails(List<string> emailIds);
        public Task<EmailMetadatas> GetEmailMetadatas(string query = "", int maxEmails = 100);
    }
    public class EmailManager : IEmailManager
    {
        private IHttpClientFactory _clientFactory;
        private GoogleApiSettings _settings;
        private IUserContextService _contextService;
        private IGoogleRequestFactory _requestFactory;

        private string _accessToken = string.Empty;
        private static readonly string _clientName = "google";

        public EmailManager(IHttpClientFactory clientFactory, GoogleApiSettings settings, IUserContextService contextService, IGoogleRequestFactory requestFactory)
        {
            _clientFactory = clientFactory;
            _settings = settings;
            _contextService = contextService;
            _requestFactory = requestFactory;
        }
        public async Task<EmailMetadatas> GetEmailMetadatas(string query = "", int maxEmails = 100)
        {
            checkAccessToken();
            HttpClient client = _clientFactory.CreateClient(_clientName);
            string queryParams = createMetadataQueryParams(query, maxEmails);
            HttpRequestMessage request = _requestFactory.CreateGetEmailIdsRequest(_accessToken, queryParams);
            HttpResponseMessage response = await client.SendAsync(request);
            string resonseText = await response.Content.ReadAsStringAsync();
            EmailMetadatas emailMetadatas = await response.Content.ReadFromJsonAsync<EmailMetadatas>() ?? new EmailMetadatas();
            return emailMetadatas;
        }

        private string createMetadataQueryParams(string query, int maxEmails)
        {
            checkAccessToken();
            StringBuilder sb = new StringBuilder();
            sb.Append("?");
            if (!string.IsNullOrEmpty(query))
            {
                sb.Append($"q={query}");
            }
            sb.Append($"maxResults={maxEmails.ToString()}");
            string queryParams = sb.ToString();
            return queryParams;
        }

        public async Task<Email> GetEmail(string emailId)
        {
            checkAccessToken();
            HttpClient client = _clientFactory.CreateClient(_clientName);
            HttpRequestMessage request = _requestFactory.CreateGetEmailRequest(_accessToken, emailId);
            HttpResponseMessage response = await client.SendAsync(request);
            Email email = await response.Content.ReadFromJsonAsync<Email>() ?? new Email();
            string responseText = await response.Content.ReadAsStringAsync();
            return email;
        }
        public async Task<List<Email>> GetEmails(List<string> emailIds)
        {
            checkAccessToken();
            List<Email> emails = new List<Email>();
            foreach (string emailId in emailIds)
            {
                Email email = await GetEmail(emailId);
                emails.Add(email);
            }
            return emails;
        }

        public void LoadAccessToken(string accessToken)
        {
            _accessToken = accessToken;
        }
        private void checkAccessToken()
        {
            if (string.IsNullOrEmpty(_accessToken)) { throw new Exception("Access token is not loaded"); }
        }

    }
}
