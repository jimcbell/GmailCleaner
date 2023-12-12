namespace GmailCleaner.Utilities
{
    public static class HttpResponseUtil
    {
        private static bool isTransientFailure(int statusCode)
        {
            return statusCode == 429 || statusCode >= 500;
        }

        public static async Task<T?> TryParseHttpResponseAsync<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                if(typeof(T) == typeof(string))
                {
                    return (T)(object)await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return await response.Content.ReadFromJsonAsync<T>();
                }
            }
            else if (isTransientFailure((int)response.StatusCode))
            {
                throw new ApplicationException($"Transient failure: {response.StatusCode}");
            }
            else
            {
                throw new Exception($"Status Code: {response.StatusCode}\nReason Phrase: {response.ReasonPhrase}");
            }
        }
        
    }
}
