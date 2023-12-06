using GmailCleaner.Common.Models;
using GmailCleaner.Managers;
using GmailCleaner.Models.Data;
using GmailCleaner.Models.ExternalModels;
using GmailCleaner.Models.Settings;
using GmailCleaner.Repositories;
using GmailCleaner.Services;
using System.Runtime;

namespace GmailCleaner.Adapters
{
    public interface IEmailAdapter
    {
        public Task<List<Email>> GetEmails(HttpRequest request, int numberEmails, string filter = "");
    }
    public class EmailsAdapter : IEmailAdapter
    {
        private IEmailRepository _emailRepository;
        private IUserContextService _userContextService;
        private IAccessTokenManager _accessManager;
        private IUserManager _userManager;


        public EmailsAdapter(IEmailRepository emailRepository, IUserContextService contextService, IAccessTokenManager accessManager, IUserManager userManager)
        {
            _emailRepository = emailRepository;
            _userContextService = contextService;
            _accessManager = accessManager;
            _userManager = userManager;

        }
        public async Task<List<Email>> GetEmails(HttpRequest request, int numberEmails, string filter = "")
        {
            GCUser user = await getUser(request);

            List<Email> emails = new List<Email>();

            string accessToken = await _accessManager.GetAccessTokenAsync(user.UserId);
            _emailRepository.LoadAccessToken(accessToken);
            EmailMetadatas emailMetadatas = await _emailRepository.GetEmailMetadatas(filter, maxEmails: numberEmails);
            List<string> emailIds = emailMetadatas.Metadatas.Select(e => e.Id).ToList();
            emails = await _emailRepository.GetEmails(emailIds);
            return emails;
        }

        private async Task<GCUser> getUser(HttpRequest request)
        {
            GmailUserModel model = _userContextService.GetUser(request);
            return await _userManager.GetUserAsync(model.UserId);

        }
    }
}
