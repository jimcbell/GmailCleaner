using Azure.Core;
using GmailCleaner.Common.Models;
using GmailCleaner.Models.ExternalModels;
using GmailCleaner.Repositories;

namespace GmailCleaner.Managers;
public interface IMessageManager
{
    public Task<List<GCMessage>> LoadMessagesAsync(GCUser user);
}

public class MessageManager : IMessageManager
{
    private IEmailManager _emailManager;
    private IMessageRepository _messageRepositoty;
    private IAccessTokenManager _accessManager;

    public MessageManager(IEmailManager emailManager, IMessageRepository messageRepository, IAccessTokenManager accessTokenManager)
    {
        _emailManager = emailManager;
        _messageRepositoty = messageRepository;
        _accessManager = accessTokenManager;
    }

    public async Task<List<GCMessage>> LoadMessagesAsync(GCUser user)
    {

        List<Email> emails = new List<Email>();

        string accessToken = await _accessManager.GetAccessTokenAsync(user.UserId);
        _emailManager.LoadAccessToken(accessToken);
        EmailMetadatas emailMetadatas = await _emailManager.GetEmailMetadatas();
        List<string> emailIds = emailMetadatas.Metadatas.Select(e => e.Id).ToList();
        emails = await _emailManager.GetEmails(emailIds);
        List<GCMessage> messages = emails.Select(e => new GCMessage(e)).ToList();
        return emails;
    }
}
