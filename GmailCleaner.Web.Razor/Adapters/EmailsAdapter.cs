using GmailCleaner.Common.Models;
using GmailCleaner.Managers;
using GmailCleaner.Models.Data;
using GmailCleaner.Models.ExternalModels;
using GmailCleaner.Models.Settings;
using GmailCleaner.Services;
using System.Runtime;

namespace GmailCleaner.Adapters
{
    public interface IEmailAdapter
    {
        /// <summary>
        /// Gets emails from the email manager.
        /// If there is a null response that means there was a transient error and it will return an empty list.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="numberEmails"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Throws an exception on a permanent api error (automatically) - should redirect to error page.</exception>"
        public Task<List<Email>?> GetEmails(GCUser user, int numberEmails, string filter = "");
    }
    public class EmailsAdapter : IEmailAdapter
    {
        private IEmailManager _emailRepository;
        private IUserContextService _userContextService;
        private IAccessTokenManager _accessManager;
        private IUserManager _userManager;


        public EmailsAdapter(IEmailManager emailRepository, IUserContextService contextService, IAccessTokenManager accessManager, IUserManager userManager)
        {
            _emailRepository = emailRepository;
            _userContextService = contextService;
            _accessManager = accessManager;
            _userManager = userManager;

        }
        public async Task<List<Email>?> GetEmails(GCUser user, int numberEmails, string filter = "")
        {
            
            List<Email> emails = new List<Email>();

            string accessToken = await _accessManager.GetAccessTokenAsync(user.UserId);
            _emailRepository.LoadAccessToken(accessToken);
            // This will throw an exception if there is a permanent error on google api, letting that be auto rethrown.
            EmailMetadatas? emailMetadatas = await _emailRepository.GetEmailMetadatas(filter, maxEmails: numberEmails);
            // If there is a transient error, it will return null, so we need to check for that.
            if(emailMetadatas != null)
            {
                List<string> emailIds = emailMetadatas.Metadatas.Select(e => e.Id).ToList();
                emails = await _emailRepository.GetEmails(emailIds, user.GmailId) ?? new();
            }
            return emails;
        }

        private async Task<GCUser> getUser(HttpRequest request)
        {
            GmailUserModel model = _userContextService.GetUser(request);
            return await _userManager.GetUserAsync(model.UserId);

        }
    }
}
