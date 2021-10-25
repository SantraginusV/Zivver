using System.Threading.Tasks;

namespace Zivver.Services
{
    /// <summary>
    /// Interface for Rest service that will be injected in class where we need to call some external API
    /// </summary>
    public interface IRestService
    {
        Task<string> GetAsync(string url);
        Task GetAsync(string url, string json);
        Task PostAsync(string url, string json);
        Task PutAsync(string url, string json);
        Task DeleteAsync(string url, string json);
    }
}
