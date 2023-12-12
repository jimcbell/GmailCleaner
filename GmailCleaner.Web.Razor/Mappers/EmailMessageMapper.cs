using GmailCleaner.Entities;
using GmailCleaner.Data.Gmail.Models;

namespace GmailCleaner.Mappers;

public interface IEmailMessageMapper
{
    public List<GCMessage> MapEmailsToMessages(List<Email> emails, GCUser user);
}
public class EmailMessageMapper: IEmailMessageMapper
{
    /// <summary>
    /// Takes a list of emails that are returned from the Gmail API and maps them to a list of GCMessage objects
    /// Only returns ones that had a list unsubscribe header
    /// Includes the user object so that the relationship is created in the database
    /// </summary>
    /// <param name="emails"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public List<GCMessage> MapEmailsToMessages(List<Email> emails, GCUser user)
    {
        List<GCMessage> messages = emails.Where(e => !string.IsNullOrEmpty(e.ListUnsubscribe)).Select(e => new GCMessage() { 
            From = e.From, 
            MessageGmailId = e.Id,
            Snippet = e.Snippet,
            UnsubscribeLink = e.UnsubscribeLink,
            User = user
        }).ToList();
        return messages;
    }
}
