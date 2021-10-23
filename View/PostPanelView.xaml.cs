using System.Windows.Controls;

namespace Zivver.View
{
    /// <summary>
    /// Interaction logic for PostPanelView.xaml
    /// </summary>
    public partial class PostPanelView : UserControl
    {
        public PostPanelView()
        {
            InitializeComponent();

            //Dispatcher.Invoke(() =>
            //{
            //    var postPanelGrid = this.FindName("postPanelGrid") as Grid;
            //    int i = 0;
            //    int j = 0;
            //    foreach (var post in posts)
            //    {
            //        var postUserControl = new PostView(
            //            new PostViewModel
            //            {
            //                Body = post.body,
            //                Id = post.id,
            //                Title = post.title,
            //                UserId = post.userId,
            //                CurrentId = post.id
            //            });
            //        postPanelGrid.Children.Add((UserControl)postUserControl);
            //        Grid.SetRow(postUserControl, i);
            //        Grid.SetColumn(postUserControl, j);
            //        i++;
            //        if (i > 9)
            //        {
            //            i = 0;
            //            j++;
            //            if (j > 9)
            //                j = 0;
            //        }
            //    }
            //});
        }
    }
}
