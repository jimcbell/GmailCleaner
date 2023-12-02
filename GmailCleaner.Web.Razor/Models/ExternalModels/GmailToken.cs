namespace GmailCleaner.Models.ExternalModels;

public record GmailToken(string AccessToken, string RefreshToken, DateTime ExpiresAt);
