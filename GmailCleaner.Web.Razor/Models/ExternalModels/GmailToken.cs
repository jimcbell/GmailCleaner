namespace GmailCleaner.Models.ExternalModels;

public record GmailToken(string AccessToken, string RefreshToken, string IdToken, DateTime ExpiresAt);
