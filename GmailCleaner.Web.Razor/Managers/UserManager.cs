using GmailCleaner.Common.Models;
using GmailCleaner.Repositories;

namespace GmailCleaner.Managers;

/// <summary>
///  Manager for interacting with users in the database and adding their access tokens
/// </summary>
public interface IUserManager
{
    public Task<GCUser?> GetUserAsync(int userId);
    public Task<GCUser?> GetUserAsync(string gmailId);
    public Task<GCUser> UpsertUserAsync(GCUser user, GCUserToken userToken);
    public Task<bool> DeleteUserAsync(int userId);
    public Task<GCUser> IncrementUserUsageAsync(GCUser user);
}

public class UserManager : IUserManager
{
    private IUserRepository _userRepository;
    private ILogger<UserManager> _logger;
    private ITokenRepository _tokenRepository;

    public UserManager(ILogger<UserManager> logger, IUserRepository userRepository, ITokenRepository tokenRepository)
    {
        _userRepository = userRepository;
        _logger = logger;
        _tokenRepository = tokenRepository;
    }
    public async Task<bool> DeleteUserAsync(int userId)
    {
        try
        {
            await _userRepository.DeleteUserAsync(userId);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error deleting user with ID {userId}");
            throw;
        }
    }

    public async Task<GCUser?> GetUserAsync(int userId)
    {
        try
        {
            GCUser? user = await _userRepository.GetUserAsync(userId);
            return user;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error deleting user with ID {userId}");
            throw;
        }
    }

    public async Task<GCUser?> GetUserAsync(string gmailId)
    {
        try
        {
            int userId = await _userRepository.GetUserIdAsync(gmailId);
            GCUser? user = await _userRepository.GetUserAsync(userId);
            return user;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error getting user with gmail ID {gmailId}");
            throw;
        }
    }

    public async Task<GCUser> UpsertUserAsync(GCUser user, GCUserToken userToken)
    {
        try
        {
            GCUser upsertedUser = await _userRepository.UpsertUserAsync(user);
            userToken.UserId = upsertedUser.UserId;
            await _tokenRepository.UpsertTokenAsync(userToken);
            return upsertedUser;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error getting user with ID {user.UserId}");
            throw;
        }
    }
    public async Task<GCUser> IncrementUserUsageAsync(GCUser user)
    {
        try
        {
            user.Usages++;
            GCUser upsertedUser = await _userRepository.UpsertUserAsync(user);
            return upsertedUser;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error getting user with ID {user.UserId}");
            throw;
        }
    }
}
