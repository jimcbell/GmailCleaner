using Azure.Core;
using GmailCleaner.Entities;
using GmailCleaner.Mappers;
using GmailCleaner.Data.Gmail.Models;
using GmailCleaner.Repositories;

namespace GmailCleaner.Managers;
/// <summary>
/// Message Manager controls the interaction with the messages in 
/// the database through the message repository as well as the email manager which interfaces with Google Apis.
/// </summary>
public interface IMessageManager
{
    /// <summary>
    /// Gets emails from email api and filters based off if they have a list unsubscribe header.
    /// If there is a null response that means there was a transient error on the email api, and must handle this accordingly.
    /// If there is a count of 0, that means there were no emails with a list unsubscribe header and is still an ok response.
    /// An exception can be thrown on permanent error, which should be handled by the caller. As the users account is likely in a bad state.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<List<GCMessage>?> LoadMessagesAsync(GCUser user);
    public Task<List<GCMessage>> GetMessagesAsync(GCUser user);
    public Task<int> TrashMessagesAsync(List<GCMessage> messages, GCUser user);
    public Task<int> DeleteAllMessagesAsync(GCUser user);
}

public class MessageManager : IMessageManager
{
    private IEmailManager _emailManager;
    private IMessageRepository _messageRepository;
    private IAccessTokenManager _accessManager;
    private IEmailMessageMapper _mapper;
    private IUserManager _userManager;
    private static readonly int _maxEmails = 50;

    public MessageManager(
        IEmailManager emailManager, 
        IMessageRepository messageRepository, 
        IAccessTokenManager accessTokenManager, 
        IEmailMessageMapper mapper,
        IUserManager userManager)
    {
        _emailManager = emailManager;
        _messageRepository = messageRepository;
        _accessManager = accessTokenManager;
        _mapper = mapper;
        _userManager = userManager;
    }

    public Task<List<GCMessage>> GetMessagesAsync(GCUser user)
    {
        return _messageRepository.GetMessagesAsync(user.UserId);
    }

    public async Task<List<GCMessage>?> LoadMessagesAsync(GCUser user)
    {
        string gmailId = user.GmailId;
        List<Email> emails = new List<Email>();
        List<GCMessage>? upsertedMessages = null;


        await getAndLoadAccessToken(user.UserId);

        // This will throw an exception if there is a permanent error on google api, letting that be auto rethrown.
        EmailMetadatas? emailMetadatas = await _emailManager.GetEmailMetadatas(user.GmailId, maxEmails: _maxEmails);
        // If there is a transient error, it will return null, so we need to check for that.
        if (emailMetadatas != null)
        {
            List<string> emailIds = emailMetadatas.Metadatas.Select(e => e.Id).ToList();
            emails = await _emailManager.GetEmails(emailIds, user.GmailId) ?? new();
        }
        if (emails.Count > 0)
        {
            List<GCMessage> messages = _mapper.MapEmailsToMessages(emails, user);
            upsertedMessages = await _messageRepository.UpsertMessagesAsync(messages);
            await _userManager.IncrementUserUsageAsync(user); // Increment the usages for the user.
        }
        return upsertedMessages;
    }

    public async Task<int> TrashMessagesAsync(List<GCMessage> messages, GCUser user)
    {
        await getAndLoadAccessToken(user.UserId);
        List<string> messageIdsDeletedInGoogle = await _emailManager.DeleteEmailsAsync(
            messages.Select(m => m.MessageGmailId).ToList(), user.GmailId);

        return await _messageRepository.DeleteMessagesAsync(messageIdsDeletedInGoogle);
    }
    private async Task getAndLoadAccessToken(int userId)
    {
        string accessToken = await _accessManager.GetAccessTokenAsync(userId);
        _emailManager.LoadAccessToken(accessToken);
    }

    public async Task<int> DeleteAllMessagesAsync(GCUser user)
    {
        return await _messageRepository.DeleteMessagesAsync(user.UserId);
    }
}
