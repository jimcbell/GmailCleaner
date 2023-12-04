namespace GmailCleaner.Models.Settings
{
    /// <summary>
    /// Settings model to easily parse settings into an object from appsettings.json
    /// </summary>
    public class GoogleApiSettings
    {
        public string RedirectUri { get; set; } = string.Empty;
        public string GrantType { get; set; } = string.Empty;
        public string GoogleAuthUrl { get; set; } = string.Empty;
        public string GoogleTokenUrl { get; set; } = string.Empty;
        public string GoogleUserInfoUrl {  get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public string MessagesRoute { get; set; } = string.Empty;   

    }
}
