﻿

using System.Reflection.Metadata.Ecma335;
using System.Web;

namespace GmailCleaner.Models.ExternalModels
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
        public MessagePart Payload { get; set; } = new();

    }
    public class MessagePart
    {
        public string PartId { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
        public List<MessagePartHeader> Headers { get; set; } = new();
        public MessagePartBody Body { get; set; } = new();
        public List<MessagePart> Parts { get; set; } = new();
    }

    public class MessagePartHeader
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class MessagePartBody
    {
        public string AttachmentId { get; set; } = string.Empty;
        public int Size { get; set; }
        private string _data = string.Empty;
        public string Data
        {
            get => _data;
            set
            {
                setData(value);
            }
        }
        public string HtmlEncodedData
        {
            get
            {
                return HttpUtility.HtmlEncode(_data);
            }
        }

        private void setData(string value)
        {
            string converted = value.Replace('-', '+');
            converted = converted.Replace('_', '/');
            byte[] base64EncodedBytes = Convert.FromBase64String(converted);
            _data = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
