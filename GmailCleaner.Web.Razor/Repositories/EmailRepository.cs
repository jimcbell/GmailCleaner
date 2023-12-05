﻿using GmailCleaner.Models.Data;
using GmailCleaner.Models.ExternalModels;
using GmailCleaner.Models.Settings;
using GmailCleaner.Services;
using System.Runtime;
using System.Text;

namespace GmailCleaner.Repositories
{
    public interface IEmailRepository
    {
        public void LoadAccessToken(string accessToken);
        public Task<GmailCleanerEmail> GetEmail(string emailId);
        public Task<List<GmailCleanerEmail>> GetEmails(List<string> emailIds);
        public Task<EmailMetadatas> GetEmailMetadatas(string query, int maxEmails = 5);
    }
    public class EmailRepository : IEmailRepository
    {
        private IHttpClientFactory _clientFactory;
        private GoogleApiSettings _settings;
        private IUserContextService _contextService;
        private IGoogleRequestFactory _requestFactory;

        private string _accessToken = string.Empty;
        private static readonly string _clientName = "google";

        public EmailRepository(IHttpClientFactory clientFactory, GoogleApiSettings settings, IUserContextService contextService, IGoogleRequestFactory requestFactory)
        {
            _clientFactory = clientFactory;
            _settings = settings;
            _contextService = contextService;
            _requestFactory = requestFactory;
        }
        public async Task<EmailMetadatas> GetEmailMetadatas(string query, int maxEmails = 5)
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

        public async Task<GmailCleanerEmail> GetEmail(string emailId)
        {
            checkAccessToken();
            HttpClient client = _clientFactory.CreateClient(_clientName);
            HttpRequestMessage request = _requestFactory.CreateGetEmailRequest(_accessToken, emailId);
            HttpResponseMessage response = await client.SendAsync(request);
            Email email = await response.Content.ReadFromJsonAsync<Email>() ?? new Email();
            GmailCleanerEmail gmailCleanerEmail = new GmailCleanerEmail(email);
            string responseText = await response.Content.ReadAsStringAsync();
            return gmailCleanerEmail;
        }
        public async Task<List<GmailCleanerEmail>> GetEmails(List<string> emailIds)
        {
            checkAccessToken();
            List<GmailCleanerEmail> emails = new List<GmailCleanerEmail>();
            foreach (string emailId in emailIds)
            {
                GmailCleanerEmail email = await GetEmail(emailId);
                emails.Add(email);
            }
            return emails;
        }

        public void LoadAccessToken(string accessToken)
        {
            this._accessToken = accessToken;
        }
        private void checkAccessToken()
        {
            if (string.IsNullOrEmpty(_accessToken)) { throw new Exception("Access token is not loaded"); }
        }

    }
}
