using GmailCleaner.Common.Models;
using GmailCleaner.Repositories;

namespace GmailCleaner.Managers
{
    /// <summary>
    /// Manager for always getting clean and fresh access tokens
    /// </summary>
    public interface IAccessTokenManager
    {
        public Task<string> GetAccessTokenAsync(int userId);

    }
    public class AccessTokenManager : IAccessTokenManager
    {
        private ITokenRepository _tokenRepository;
        private ILogger<UserManager> _logger;

        public AccessTokenManager(ILogger<UserManager> logger, ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
            _logger = logger;
        }
        public async Task<string> GetAccessTokenAsync(int userId)
        {
            try
            {
                _logger.LogInformation($"Getting access token for user with ID {userId}");
                GCUserToken token = await _tokenRepository.GetTokenByUserIdAsync(userId);
                if(token.ExpiresOn > DateTime.Now - TimeSpan.FromMinutes(5))
                {
                    _logger.LogInformation($"Token for user with ID {userId} is still valid");
                }
                else
                {
                    GCUserToken newToken = await _tokenRepository.GetNewTokenAsync(token.RefreshToken!);
                    newToken.UserId = userId;
                    token = await _tokenRepository.UpsertTokenAsync(newToken);
                    _logger.LogInformation($"Token for user with ID {userId} has been refreshed");
                }

                return token.AccessToken!;

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error getting access token for user with ID {userId}");
                throw;
            }
        }

    }
}
