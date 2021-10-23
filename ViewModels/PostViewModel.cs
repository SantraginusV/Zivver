using System.Collections.ObjectModel;
using System.Windows.Input;
using Zivver.Helpers;
using Zivver.View;

namespace Zivver.ViewModels
{
    public class PostViewModel : ViewModelBase
    {
        public PostViewModel()
        {
            ChangeCurrentIdsCommand = new RelayCommand<PostPanelView>(ChangeCurrentIds, CanChangeCurrentIds);
        }

        private int _id;
        public int Id
        {
            get { return _id; }
            set 
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        private int _userId;
        public int UserId
        {
            get { return _userId; }
            set
            {
                _userId = value;
                OnPropertyChanged();
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _body;
        public string Body
        {
            get { return _body; }
            set
            {
                _body = value;
                OnPropertyChanged();
            }
        }

        private ICommand _changeCurrentIdsCommand;
        public ICommand ChangeCurrentIdsCommand
        {
            get { return _changeCurrentIdsCommand; }
            set { _changeCurrentIdsCommand = value; }
        }

        private void ChangeCurrentIds(PostPanelView postPanel)
        {
            foreach(var post in (ObservableCollection<PostViewModel>)postPanel.DataContext)
            {
                var temp = post.Id;
                post.Id = post.UserId;
                post.UserId = temp;
            }
        }

        private bool CanChangeCurrentIds(PostPanelView postPanel)
        {
            return true;
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
