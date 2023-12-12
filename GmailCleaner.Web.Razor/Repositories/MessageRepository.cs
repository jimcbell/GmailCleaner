﻿using GmailCleaner.Data;
using GmailCleaner.Entities;
using Microsoft.EntityFrameworkCore;

namespace GmailCleaner.Repositories;

public interface IMessageRepository
{
    public Task<GCMessage> UpsertMessageAsync(GCMessage message);
    public Task<List<GCMessage>> UpsertMessagesAsync(List<GCMessage> messages);
    public Task<GCMessage> GetMessageAsync(int messageId);
    public Task<List<GCMessage>> GetMessagesAsync(int userId);
    public Task<bool> DeleteMessageAsync(int messageId);
    public Task<int> DeleteMessagesAsync(int userId);
    public Task<int> DeleteMessagesAsync(List<string> gmailMessageIds);
}
public class MessageRepository : IMessageRepository
{
    private GmailCleanerContext _context;

    public MessageRepository(GmailCleanerContext context)
    {
        _context = context;
    }
    public Task<bool> DeleteMessageAsync(int messageId)
    {
        throw new NotImplementedException();
    }

    public async Task<int> DeleteMessagesAsync(int userId)
    {
        List<GCMessage> messages = _context.GCMessages.Where(m => m.User.UserId == userId).ToList();
        _context.GCMessages.RemoveRange(messages);
        return await _context.SaveChangesAsync();
    }

    public async Task<int> DeleteMessagesAsync(List<string> gmailMessageIds)
    {
        List<GCMessage> messages = await _context.GCMessages.Where(m => gmailMessageIds.Contains(m.MessageGmailId)).ToListAsync();
        _context.GCMessages.RemoveRange(messages);
        return await _context.SaveChangesAsync();
    }

    public async Task<GCMessage> GetMessageAsync(int messageId)
    {
        GCMessage? message = await _context.GCMessages.Where(m => m.MessageId == messageId).FirstOrDefaultAsync();
        if (message == null)
        {
            throw new Exception($"Message with ID {messageId} not found");
        }
        else
        {
            return message;
        }
    }

    public async Task<List<GCMessage>> GetMessagesAsync(int userId)
    {
        List<GCMessage> messages = await  _context.GCMessages.Where(m => m.User.UserId == userId).ToListAsync();
        return messages;
    }

    public Task<GCMessage> UpsertMessageAsync(GCMessage message)
    {
        throw new NotImplementedException();
    }

    public async Task<List<GCMessage>> UpsertMessagesAsync(List<GCMessage> messages)
    {
        List<GCMessage> upsertedMessages = new();
        foreach(GCMessage message in messages)
        {
            GCMessage? existingMessage = await _context.GCMessages.Where(m => m.MessageGmailId == message.MessageGmailId).FirstOrDefaultAsync();
            if (existingMessage == null)
            {
                _context.GCMessages.Add(message);
                existingMessage = message;
            }
            else
            {
                existingMessage.Snippet = message.Snippet;
                existingMessage.UnsubscribeLink = message.UnsubscribeLink;
                existingMessage.From = message.From;
            }
            upsertedMessages.Add(existingMessage);
        }
        await _context.SaveChangesAsync();
        return upsertedMessages;
    }
}
