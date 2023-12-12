namespace GmailCleaner.Data.Gmail.Models;

public record GmailToken(string AccessToken, string RefreshToken, string IdToken, DateTime ExpiresAt);
