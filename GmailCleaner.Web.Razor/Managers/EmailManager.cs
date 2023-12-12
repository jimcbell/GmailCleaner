using Azure;
using GmailCleaner.Data.Gmail.Models;
using GmailCleaner.Data.Settings;
using GmailCleaner.Services;
using GmailCleaner.Data.Utilities;
using Microsoft.Identity.Client;
using System.Runtime;
using System.Text;

namespace GmailCleaner.Managers
{
    /// <summary>
    /// This manager is used to interact with the google apis for emails.
    /// </summary>
    public interface IEmailManager
    {
        public void LoadAccessToken(string accessToken);
        /// <summary>
        /// Gets the email for the given email id.
        /// If the response is a transient error, it will eat the exception and return null.
        /// If the response is a permanent error, it will throw an Exception to be handled by the caller.
        /// </summary>
        /// <param name="emailId"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Throws an exception on a permanent api error</exception>
        public Task<Email?> GetEmail(string emailId, string gmailId);
        /// <summary>
        /// Takes the emails provided in the list and calls the singular GetEmail method.
        /// If it recieves a null response from GetEmail, it means there was a transient error and it will stop executing on the list.
        /// To maintain high availability, it will return the emails that were able to be successfully retrieved up to the point of transient error.
        /// To maintain high reliability, it will not attempt to wait in case of a 429, it will just return the emails that were able to be successfully retrieved up to the point of transient error.
        /// If no emails were successfully retrieved, it will return an empty list.
        /// </summary>
        /// <param name="emailIds"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Throws an exception on a permanent api error</exception>
        public Task<List<Email>> GetEmails(List<string> emailIds, string gmailId);
        /// <summary>
        /// Gets email metadatas for a given query.
        /// If the response is a transient error, it will eat the exception and return null.
        /// Any permanent api errors will throw an exception, ex 401.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="maxEmails"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Throws an exception on a permanent api error</exception>
        public Task<EmailMetadatas?> GetEmailMetadatas(string gmailId, string query = "", int maxEmails = 50);
        /// <summary>
        /// Deletes emails from users inbox.
        /// Returns the gmail ids of those that were succesfully deleted.
        /// </summary>
        /// <param name="emailMessageIds"></param>
        /// <param name="gmailId"></param>
        /// <returns></returns>
        public Task<List<string>> DeleteEmailsAsync(List<string> emailMessageIds, string gmailId);  
        public Task<string?> DeleteEmailAsync(string emailMessageId, string gmailId);  
    }
    public class EmailManager : IEmailManager
    {
        private IHttpClientFactory _clientFactory;
        private GoogleApiSettings _settings;
        private IUserContextService _contextService;
        private IGoogleRequestFactory _requestFactory;
        private ILogger<EmailManager> _logger;
        private string _accessToken = string.Empty;
        private static readonly string _clientName = "google";

        public EmailManager(
            IHttpClientFactory clientFactory,
            GoogleApiSettings settings,
            IUserContextService contextService,
            IGoogleRequestFactory requestFactory,
            ILogger<EmailManager> logger)
        {
            _clientFactory = clientFactory;
            _settings = settings;
            _contextService = contextService;
            _requestFactory = requestFactory;
            _logger = logger;
        }
        public async Task<EmailMetadatas?> GetEmailMetadatas(string gmailId, string query = "", int maxEmails = 50)
        {
            EmailMetadatas? emailMetadatas = null;
            checkAccessToken();
            HttpClient client = _clientFactory.CreateClient(_clientName);
            string queryParams = createMetadataQueryParams(query, maxEmails);
            HttpRequestMessage request = _requestFactory.CreateGetEmailIdsRequest(_accessToken, queryParams, gmailId);
            HttpResponseMessage response = await client.SendAsync(request);
            try
            {
                emailMetadatas = await GetResponseOrDefault<EmailMetadatas>(response, gmailId);
            }
            catch
            {
                throw; // Rethrow exception to be handled by caller if permanent error.
            }
            //string resonseText = await response.Content.ReadAsStringAsync();
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
        /// <summary>
        /// Uses the HttpResponseUtil to get the response from the HttpResponseMessage.
        /// This has logic to determine the type of error permanent/transiet.
        /// If success will return the deserialized response.
        /// This will log any errors.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpResponseMessage"></param>
        /// <returns></returns>
        private async Task<T?> GetResponseOrDefault<T>(HttpResponseMessage httpResponseMessage, string id)
        {
            T? response = default;
            try
            {
                // Success will return the deserialized T response from the HttpResponseMessage.
                response = await HttpResponseUtil.TryParseHttpResponseAsync<T>(httpResponseMessage);
            }
            // Transient error will return null response.
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            // Permanent error will throw an exception.
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"Permanent error getting email metadata for user id: {id}");
                sb.Append(ex.Message);
                _logger.LogError(ex, sb.ToString());
                throw;
            }
            return response;
        }

        public async Task<Email?> GetEmail(string emailId, string gmailId)
        {
            Email? email = null;
            checkAccessToken();
            HttpClient client = _clientFactory.CreateClient(_clientName);
            HttpRequestMessage request = _requestFactory.CreateGetEmailRequest(_accessToken, emailId, gmailId);
            HttpResponseMessage response = await client.SendAsync(request);
            try
            {
                email = await GetResponseOrDefault<Email>(response, gmailId);
            }
            catch
            {
                throw; // Rethrow exception to be handled by caller if permanent error.
            }
            //string responseText = await response.Content.ReadAsStringAsync();
            return email;
        }
        public async Task<List<Email>> GetEmails(List<string> emailIds, string gmailId)
        {
            List<Email> emails = new();
            checkAccessToken();
            foreach (string emailId in emailIds)
            {
                try
                {
                    Email? email = await GetEmail(emailId, gmailId);
                    if (email != null)
                    {
                        emails.Add(email);
                    }
                    else
                    {
                        break;
                    }
                }
                catch
                {
                    throw; // Rethrow exception to be handled by caller if permanent error.
                }
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

        public async Task<List<string>> DeleteEmailsAsync(List<string> emailIds, string gmailId)
        {
            checkAccessToken();
            List<string> deletedEmails = new();
            foreach(string emailId in emailIds)
            {
                try
                {
                    string? response = await DeleteEmailAsync(emailId, gmailId);
                    if(response != null)
                    {
                            _logger.LogInformation($"Deleted email with id: {emailId} for user: {gmailId}");
                            deletedEmails.Add(emailId);
                    }
                    else
                    {
                        _logger.LogError($"Failed to delete email with id: {emailId} for user: {gmailId}");
                        break;
                    }
                }
                catch
                {
                    throw; // Rethrow exception to be handled by caller if permanent error.
                }
            }
            return deletedEmails;
        }

        public async Task<string?> DeleteEmailAsync(string emailId, string gmailId)
        {
            checkAccessToken();
            string? responseText = null;
            HttpClient client = _clientFactory.CreateClient(_clientName);
            HttpRequestMessage request = _requestFactory.CreateDeleteEmailRequest(_accessToken, emailId, gmailId);
            HttpResponseMessage response = await client.SendAsync(request);
            try
            {
                responseText = await GetResponseOrDefault<string>(response, gmailId);
            }
            catch
            {
                throw; // Rethrow exception to be handled by caller if permanent error.
            }
            return responseText;
        }
    }
}
