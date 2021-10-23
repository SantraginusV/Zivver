using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Zivver.Services
{
    // Error handling will be done where rest service is used
    public class RestService : IRestService
    {
        private static HttpClient client;

        public RestService()
        {
            client = new HttpClient
            {
                Timeout = new TimeSpan(0, 0, 15)
            };
        }

        public async Task<string> GetAsync(string url)
        {
            HttpResponseMessage reponse = await client.GetAsync(new Uri(string.Format(url, string.Empty)));
            string jsonResponse = await reponse.Content.ReadAsStringAsync();
            return jsonResponse;
        }

        public Task GetAsync(string url, string json)
        {
            throw new NotImplementedException();
        }

        public Task PutAsync(string url, string json)
        {
            throw new NotImplementedException();
        }

        public Task PostAsync(string url, string json)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string url, string id)
        {
            throw new NotImplementedException();
        }
    }
}
