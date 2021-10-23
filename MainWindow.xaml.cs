using System.Windows;
using Zivver.ViewModels;

namespace Zivver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(PostPanelViewModel dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
        }
    }
}
