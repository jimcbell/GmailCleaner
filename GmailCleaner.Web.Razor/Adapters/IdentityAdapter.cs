using Azure.Core;
using GmailCleaner.Entities;
using GmailCleaner.Managers;
using GmailCleaner.Data.Gmail.Models;
using GmailCleaner.Services;

namespace GmailCleaner.Adapters
{
    public interface IIdentityAdapter
    {
        public Task<GCUser?> GetUser(HttpRequest request);
    }
    public class IdentityAdapter : IIdentityAdapter
    {
        private IUserManager _userManager;
        private ILogger<IdentityAdapter> _logger;
        private IUserContextService _contextService;

        public IdentityAdapter(ILogger<IdentityAdapter> logger, IUserManager userManager, IUserContextService contextService)
        {
            _userManager = userManager;
            _logger = logger;
            _contextService = contextService;
        }
        public async Task<GCUser?> GetUser(HttpRequest request)
        {
            GmailUserModel gmailUser = _contextService.GetUser(request);
            GCUser? user = null;
            try
            {
                user = await _userManager.GetUserAsync(gmailUser.UserId);
                return user;
            }
            catch(Exception e)
            {
                _logger.LogError(e, $"Error getting user with gmail ID {gmailUser.UserId}");
                throw;
            }


        }
    }
}
