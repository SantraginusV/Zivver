using System.Threading.Tasks;

namespace Zivver.Services
{
    public interface IRestService
    {
        Task<string> GetAsync(string url);
        Task GetAsync(string url, string json);
        Task PostAsync(string url, string json);
        Task PutAsync(string url, string json);
        Task DeleteAsync(string url, string json);
    }
}
