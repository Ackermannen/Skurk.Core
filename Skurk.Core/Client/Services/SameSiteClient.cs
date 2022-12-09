namespace Skurk.Core.Client.Services
{
    public class SameSiteClient
    {
        private readonly HttpClient _client;

        public SameSiteClient(HttpClient client)
        {
            _client = client;
        }

    }
}
