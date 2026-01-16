using MahApps.Metro.Controls;
using _3_project.ViewModels;

namespace _3_project
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
