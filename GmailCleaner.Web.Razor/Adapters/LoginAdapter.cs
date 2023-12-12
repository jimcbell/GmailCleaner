using Azure.Core;
using GmailCleaner.Entities;
using GmailCleaner.Managers;
using GmailCleaner.Data.Gmail.Models;
using GmailCleaner.Repositories;
using GmailCleaner.Services;
using Microsoft.Extensions.Logging;

namespace GmailCleaner.Adapters
{
    public interface ILoginAdapter
    {
        //public Task<int> AddUser(GmailUserModel gmailUserModel, GmailToken gmailToken);
        public Task<GCUser> LogInUser(HttpRequest request);
    }
    public class LoginAdapter : ILoginAdapter
    {
        private IUserManager _userManager;
        private IUserContextService _contextService;
        private ILogger<LoginAdapter> _logger;

        public LoginAdapter(ILogger<LoginAdapter> logger, IUserManager userManager, IUserContextService contextService)
        {
            _userManager = userManager;
            _contextService = contextService;
            _logger = logger;
        }
        //public async Task<int> AddUser(GmailUserModel gmailUserModel, GmailToken gmailToken)
        //{
        //    GCUser user = new()
        //    {
        //        Name = gmailUserModel.Name,
        //        Email = gmailUserModel.Email,
        //        GmailId = gmailUserModel.UserId,
        //    };
        //    GCUserToken userToken = new()
        //    {
        //        AccessToken = gmailToken.AccessToken,
        //        RefreshToken = gmailToken.RefreshToken,
        //        ExpiresOn = gmailToken.ExpiresAt
        //    };
        //    user.GCUserTokens.Add(userToken);
        //    GCUser dbUser = await _userRepository.UpsertUserAsync(user);
        //    await _userRepository.UpsertTokenAsync(dbUser, userToken);
        //    return dbUser.UserId;
        //}

        public async Task<GCUser> LogInUser(HttpRequest request)
        {
            _logger.LogDebug("Logging in user");
            GmailToken gmailToken = _contextService.GetToken(request);
            GmailUserModel gmailUser = _contextService.GetUser(request);
            GCUser user = parseUser(gmailUser);
            GCUserToken userToken = parseToken(gmailToken);
            try
            {
                
                _logger.LogInformation($"Logging in user with gmail id: {user.GmailId}");
                GCUser loggedInUser = await _userManager.UpsertUserAsync(user, userToken);
                _logger.LogInformation($"Succesfully logged in user with id: {loggedInUser.UserId}");
                return loggedInUser;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error logging in user with gmail id: {user.GmailId}");
                throw;
            }
            

        }
        private GCUser parseUser(GmailUserModel gmailUserModel)
        {
            GCUser user = new()
            {
                Name = gmailUserModel.Name,
                Email = gmailUserModel.Email,
                GmailId = gmailUserModel.UserId,
            };
            return user;
        }
        private GCUserToken parseToken(GmailToken gmailToken)
        {
            GCUserToken userToken = new()
            {
                AccessToken = gmailToken.AccessToken,
                RefreshToken = gmailToken.RefreshToken,
                ExpiresOn = gmailToken.ExpiresAt,
                IdToken = gmailToken.IdToken
            };
            return userToken;
        }
    }
}
