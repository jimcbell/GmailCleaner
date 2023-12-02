using GmailCleaner.Models.Data;
using GmailCleaner.Models.Settings;
using GmailCleaner.Repositories;
using GmailCleaner.Services;
using System.Runtime;

namespace GmailCleaner.Adapters
{
    public interface IEmailAdapter
    {
        public Task<List<Email>> GetEmails(string accessToken, string filter = "");
    }
    public class EmailsAdapter : IEmailAdapter
    {
        private IEmailRepository _emailRepository;
        private IUserContextService _userContextService;

        public EmailsAdapter(IEmailRepository emailRepository, IUserContextService contextService)
        {
            _emailRepository = emailRepository;
            _userContextService = contextService;

        }
        public async Task<List<Email>> GetEmails(string accessToken, string filter = "")
        {
            List<Email> emails = new List<Email>();
            _emailRepository.LoadAccessToken(accessToken);
            EmailMetadatas emailMetadatas = await _emailRepository.GetEmailMetadatas(filter);
            List<string> emailIds = emailMetadatas.Metadatas.Select(e => e.Id).ToList();
            emails = await _emailRepository.GetEmails(emailIds);
            return emails;
        }
    }
}
