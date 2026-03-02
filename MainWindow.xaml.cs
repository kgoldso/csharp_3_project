using System.Windows;
using _3_project.ViewModels;

namespace _3_project
{
    public partial class MainWindow
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();

            // Устанавливаем DataContext сразу через DI
            DataContext = viewModel;

            Loaded += OnWindowLoaded;
            viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
                UpdateColumnHeaders(vm);
        }

        private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.SelectedLanguage) && DataContext is MainViewModel vm)
                UpdateColumnHeaders(vm);
        }

        private void UpdateColumnHeaders(MainViewModel vm)
        {
            PeopleGrid.Columns[0].Header = vm.ColId;
            PeopleGrid.Columns[1].Header = vm.ColDate;
            PeopleGrid.Columns[2].Header = vm.ColFirstName;
            PeopleGrid.Columns[3].Header = vm.ColLastName;
            PeopleGrid.Columns[4].Header = vm.ColSurName;
            PeopleGrid.Columns[5].Header = vm.ColCity;
            PeopleGrid.Columns[6].Header = vm.ColCountry;
        }
    }
}
