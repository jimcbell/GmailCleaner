using GmailCleaner.Models.ExternalModels;
using System.Web;

namespace GmailCleaner.Models.Data
{
    public class GmailCleanerEmail
    {
        //private static readonly string HTML = "text/html";
        //private static readonly string PLAIN = "text/plain";
        public string Id { get; set; } = string.Empty;
        public string ThreadId { get; set; } = string.Empty;
        public string Snippet { get; set; } = string.Empty;
        public string InternalDate { get; set; } = string.Empty;        
        public string From { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string ListUnsubscribe { get; set; } = string.Empty;
        public string OrginalSender { get; set; } = string.Empty;
        public bool IsHtml { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string HtmlBody { get; set; } = string.Empty;    
        public string PlainBody { get; set; } = string.Empty;
        public bool HasListUnsubscribe
        {
            get
            {
                return !string.IsNullOrEmpty(ListUnsubscribe);
            }
        }
        public GmailCleanerEmail(Email email)
        {
            this.Id = email.Id;
            this.ThreadId = email.ThreadId;
            this.Snippet = email.Snippet;
            this.InternalDate = email.InternalDate;
            this.From = email.Payload.Headers.Where(h => h.Name == "From").FirstOrDefault()?.Value ?? string.Empty;
            this.Subject = email.Payload.Headers.Where(h => h.Name == "Subject").FirstOrDefault()?.Value ?? string.Empty;
            this.ListUnsubscribe = email.Payload.Headers.Where(h => h.Name == "List-Unsubscribe").FirstOrDefault()?.Value ?? string.Empty;
            //this.ContentType = email.Payload.MimeType;
            
            //this.HtmlBody = email.Payload.Parts.Where(p => p.MimeType == "text/html").FirstOrDefault()?.Body.Data ?? string.Empty;
            //this.PlainBody = email.Payload.Parts.Where(p => p.MimeType == "text/plain").FirstOrDefault()?.Body.Data ?? string.Empty;
        }
    }

}
