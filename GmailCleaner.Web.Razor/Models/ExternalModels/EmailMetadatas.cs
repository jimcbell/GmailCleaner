using System.Text.Json.Serialization;

namespace GmailCleaner.Models.ExternalModels
{
    public class EmailMetadatas
    {
        [JsonPropertyName("messages")]
        public List<EmailMetadata> Metadatas { get; set; } = new List<EmailMetadata>();
    }

    public class EmailMetadata
    {
        public string Id { get; set; } = string.Empty;
        public string ThreadId { get; set; } = string.Empty;
    }
}
