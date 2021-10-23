using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Zivver.Services;

namespace Zivver.ViewModels
{
    public class PostPanelViewModel : ViewModelBase
    {
        private ILogger<PostPanelViewModel> _log;
        private IConfiguration _config;
        private IRestService _client;

        public PostPanelViewModel(ILogger<PostPanelViewModel> log, IConfiguration config, IRestService client)
        {
            _log = log;
            _config = config;
            _client = client;
        }

        private ObservableCollection<PostViewModel> _post;
        public ObservableCollection<PostViewModel> Posts
        {
            get 
            { 
                if(_post == null)
                {
                    _post = new ObservableCollection<PostViewModel>();
                    LoadPosts();
                }
                return _post; 
            }
            set
            {
                _post = value;
                OnPropertyChanged();
            }
        }

        private void LoadPosts()
        {
            //check this Thread.Run
            _client.GetAsync(_config["URL"]).ContinueWith(task =>
            {
                if (task.Exception == null)
                {
                    var result = task.Result;
                    Posts = JsonConvert.DeserializeObject<ObservableCollection<PostViewModel>>(result);
                }
                else
                {
                    _log.LogError(task.Exception.Message);
                }    
            });
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
