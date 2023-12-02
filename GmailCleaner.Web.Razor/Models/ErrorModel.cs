namespace GmailCleaner.Models
{
    public class ErrorModel
    {
        public List<Error> Errors { get; set; } = new List<Error>();
        public int Count { get => Errors.Count; }
    }

    public class Error
    {
        public int? StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public Error(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
        public Error(string message)
        {
            Message = message;
        }
    }
}
