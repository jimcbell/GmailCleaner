using GmailCleaner.Common.Models;
using GmailCleaner.Models.ExternalModels;
using GmailCleaner.Repositories;

namespace GmailCleaner.Adapters
{
    public interface ILoginAdapter
    {
        public Task<int> AddUser(GmailUserModel gmailUserModel, GmailToken gmailToken);
    }
    public class LoginAdapter : ILoginAdapter
    {
        private IUserRepository _userRepository;

        public LoginAdapter(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<int> AddUser(GmailUserModel gmailUserModel, GmailToken gmailToken)
        {
            GCUser user = new()
            {
                Name = gmailUserModel.Name,
                Email = gmailUserModel.Email,
                GmailId = gmailUserModel.UserId,
            };
            GCUserToken userToken = new()
            {
                AccessToken = gmailToken.AccessToken,
                RefreshToken = gmailToken.RefreshToken,
                ExpiresOn = gmailToken.ExpiresAt
            };
            user.GCUserTokens.Add(userToken);
            GCUser dbUser = await _userRepository.UpsertUserAsync(user);
            await _userRepository.UpsertTokenAsync(dbUser, userToken);
            return dbUser.UserId;
        }
    }
}
