namespace GmailCleaner.Models.Data
{
    public class Email
    {
        public string Id { get; set; } = string.Empty;
        public string ThreadId { get; set; } = string.Empty;
        public List<string> LabelIds { get; set; } = new();
        public string Snippet { get; set; } = string.Empty;

        public int SizeEstimate { get; set; }
        public string HistoryId { get; set; } = string.Empty;
        public string InternalDate { get; set; } = string.Empty;

    }
}
