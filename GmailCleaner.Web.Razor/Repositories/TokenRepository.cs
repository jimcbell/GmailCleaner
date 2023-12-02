using GmailCleaner.Common;
using GmailCleaner.Common.Models;
using GmailCleaner.Models.ExternalModels;
using GmailCleaner.Models.Settings;
using GmailCleaner.Services;
using Microsoft.EntityFrameworkCore;

namespace GmailCleaner.Repositories
{
    public interface ITokenRepository
    {
        public Task<GCUserToken> GetTokenAsync(int tokenId);
        public Task<GCUserToken> UpsertTokenAsync(GCUserToken token);
        public Task<bool> DeleteTokenAsync(int tokenId);
        public Task<GCUserToken> GetNewTokenAsync(string refreshToken);
    }
    public class TokenRepository : ITokenRepository
    {
        private GmailCleanerContext _context;
        private GoogleApiSettings _settings;
        private IHttpClientFactory _clientFactory;
        private IGoogleRequestFactory _requestFactory;

        public TokenRepository(GmailCleanerContext context, GoogleApiSettings settings, IHttpClientFactory clientFactory, IGoogleRequestFactory requestFactory)
        {
            _context = context;
            _settings = settings;
            _clientFactory = clientFactory;
            _requestFactory = requestFactory;
        }
        public async Task<bool> DeleteTokenAsync(int tokenId)
        {

                GCUserToken? token = _context.GCUserTokens.Where(t => t.UserTokenId == tokenId).FirstOrDefault<GCUserToken>();
                if (token == null)
                {
                    throw new Exception($"Token with ID {tokenId} not found");
                }
                else
                {
                    _context.GCUserTokens.Remove(token);
                }
                await _context.SaveChangesAsync();
                return true;

        }

        public async Task<GCUserToken> GetNewTokenAsync(string refreshToken)
        {
            HttpClient client = _clientFactory.CreateClient("google-auth");
            HttpRequestMessage request = _requestFactory.CreateRefreshTokenRequest(refreshToken);
            HttpResponseMessage response = await client.SendAsync(request);
            RefreshResponse refreshResponse = await response.Content.ReadFromJsonAsync<RefreshResponse>() ?? new RefreshResponse();
            GCUserToken token = new GCUserToken()
            {
                AccessToken = refreshResponse.AccessToken,
                ExpiresOn = DateTime.Now + TimeSpan.FromSeconds(refreshResponse.ExpiresIn),
                RefreshToken = refreshToken,
                IdToken = ""
            };
            return new GCUserToken();
        }

        public async Task<GCUserToken> GetTokenAsync(int tokenId)
        {

                GCUserToken? token = await _context.GCUserTokens.Where(t => t.UserTokenId == tokenId).FirstOrDefaultAsync<GCUserToken>();
                if (token == null || token.AccessToken == null || token.RefreshToken == null)
                {
                    throw new Exception($"Token with ID {tokenId} not found or has corrupted data");
                }
                return token;
        }

        public async Task<GCUserToken> UpsertTokenAsync(GCUserToken token)
        {

            GCUserToken? userToken = await _context.GCUserTokens.Where(t => t.UserId == token.UserId).FirstOrDefaultAsync<GCUserToken>();
            if (userToken == null)
            {
                userToken = token;
                await _context.GCUserTokens.AddAsync(userToken);
            }
            else
            {
                userToken.ExpiresOn = token.ExpiresOn;
                userToken.AccessToken = token.AccessToken;
                if (!string.IsNullOrEmpty(token.RefreshToken))
                {
                    userToken.RefreshToken = token.RefreshToken;
                }
                userToken.IdToken = token.IdToken;
            }
            await _context.SaveChangesAsync();
            return userToken;
        }
    }
}
