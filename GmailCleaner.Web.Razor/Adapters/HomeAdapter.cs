using Azure.Core;
using GmailCleaner.Common.Models;
using GmailCleaner.Managers;
using GmailCleaner.Models.ExternalModels;
using GmailCleaner.Services;

namespace GmailCleaner.Adapters
{
    public interface IHomeAdapter
    {
        public Task<GCUser?> GetUser(HttpRequest request);
    }
    public class HomeAdapter : IHomeAdapter
    {
        private IUserManager _userManager;
        private ILogger<HomeAdapter> _logger;
        private IUserContextService _contextService;

        public HomeAdapter(ILogger<HomeAdapter> logger, IUserManager userManager, IUserContextService contextService)
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
            }
            return user;


        }
    }
}
