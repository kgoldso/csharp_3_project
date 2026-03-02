using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Win32;
using _3_project.Helpers;
using _3_project.Models;
using _3_project.Repositories;
using _3_project.Services;

namespace _3_project.ViewModels
{
    /// <summary>
    /// ViewModel для главного окна приложения.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private readonly IPersonRepository _repository;
        private readonly CsvImportService _csvImportService;
        private readonly IExportService _excelExportService;
        private readonly IExportService _xmlExportService;

        // Фильтры
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _city = string.Empty;
        private string _country = string.Empty;
        private DateTime? _filterDate;

        // Статус и данные
        private string _statusMessage = string.Empty;
        private bool _isLoading;
        private ObservableCollection<Person> _searchResults = [];

        // Язык
        private string _selectedLanguage = "en";

        public MainViewModel(
            IPersonRepository repository,
            CsvImportService csvImportService,
            ExcelExportService excelExportService,
            XmlExportService xmlExportService)
        {
            _repository = repository;
            _csvImportService = csvImportService;
            _excelExportService = excelExportService;
            _xmlExportService = xmlExportService;

            // Инициализация команд
            ImportCommand      = new AsyncRelayCommand(ImportAsync,      _ => !IsLoading);
            SearchCommand      = new AsyncRelayCommand(SearchAsync,      _ => !IsLoading);
            ExportExcelCommand = new AsyncRelayCommand(ExportExcelAsync, _ => !IsLoading && SearchResults.Count > 0);
            ExportXmlCommand   = new AsyncRelayCommand(ExportXmlAsync,   _ => !IsLoading && SearchResults.Count > 0);
        }

        #region Properties

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }

        public string Country
        {
            get => _country;
            set => SetProperty(ref _country, value);
        }

        public DateTime? FilterDate
        {
            get => _filterDate;
            set => SetProperty(ref _filterDate, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ObservableCollection<Person> SearchResults
        {
            get => _searchResults;
            set => SetProperty(ref _searchResults, value);
        }

        public IReadOnlyList<string> AvailableLanguages { get; } = ["en", "ru"];

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                SetProperty(ref _selectedLanguage, value);
                CultureInfo.CurrentUICulture = new CultureInfo(value);

                // Меняем ResourceDictionary для UI
                var dict = new ResourceDictionary
                {
                    Source = new Uri($"Resources/Strings.{value}.xaml", UriKind.Relative)
                };
                var existing = Application.Current.Resources.MergedDictionaries
                    .FirstOrDefault(d => d.Source?.OriginalString.Contains("Strings.") == true);
                if (existing != null)
                    Application.Current.Resources.MergedDictionaries.Remove(existing);
                Application.Current.Resources.MergedDictionaries.Add(dict);

                // Обновляем заголовки колонок DataGrid
                OnPropertyChanged(nameof(ColId));
                OnPropertyChanged(nameof(ColDate));
                OnPropertyChanged(nameof(ColFirstName));
                OnPropertyChanged(nameof(ColLastName));
                OnPropertyChanged(nameof(ColSurName));
                OnPropertyChanged(nameof(ColCity));
                OnPropertyChanged(nameof(ColCountry));
            }
        }

        // Заголовки колонок — читаются из активного ResourceDictionary
        public string ColId        => GetResource("ColId");
        public string ColDate      => GetResource("ColDate");
        public string ColFirstName => GetResource("ColFirstName");
        public string ColLastName  => GetResource("ColLastName");
        public string ColSurName   => GetResource("ColSurName");
        public string ColCity      => GetResource("ColCity");
        public string ColCountry   => GetResource("ColCountry");

        private static string GetResource(string key) =>
            Application.Current.Resources[key] as string ?? key;

        #endregion

        #region Commands

        public AsyncRelayCommand ImportCommand { get; }
        public AsyncRelayCommand SearchCommand { get; }
        public AsyncRelayCommand ExportExcelCommand { get; }
        public AsyncRelayCommand ExportXmlCommand { get; }

        #endregion

        #region Methods

        private async Task ImportAsync(object? parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                Title  = "Select CSV file to import"
            };

            if (openFileDialog.ShowDialog() != true)
                return;

            IsLoading     = true;
            StatusMessage = "Importing...";

            try
            {
                var peopleStream = _csvImportService.ImportFromCsvAsync(openFileDialog.FileName);
                await _repository.AddRangeAsync(peopleStream, batchSize: 1000);

                var totalCount = await _repository.GetCountAsync();
                StatusMessage = $"✓ Import complete! Total records: {totalCount}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Error: {ex.Message}";
                MessageBox.Show($"Import failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SearchAsync(object? parameter)
        {
            IsLoading     = true;
            StatusMessage = "Searching...";

            try
            {
                var results = await _repository.SearchAsync(
                    FirstName, LastName, City, Country, FilterDate);

                SearchResults = new ObservableCollection<Person>(results);
                StatusMessage = $"Found {results.Count} records";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Search error: {ex.Message}";
                MessageBox.Show($"Search failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExportExcelAsync(object? parameter)
        {
            IsLoading     = true;
            StatusMessage = "Exporting to Excel...";

            try
            {
                var filePath = await _excelExportService.ExportAsync(SearchResults.AsAsyncEnumerable());
                StatusMessage = $"✓ Exported to Excel";
                MessageBox.Show($"Exported successfully!\n{filePath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Export error: {ex.Message}";
                MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExportXmlAsync(object? parameter)
        {
            IsLoading     = true;
            StatusMessage = "Exporting to XML...";

            try
            {
                var filePath = await _xmlExportService.ExportAsync(SearchResults.AsAsyncEnumerable());
                StatusMessage = $"✓ Exported to XML";
                MessageBox.Show($"Exported successfully!\n{filePath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Export error: {ex.Message}";
                MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion
    }
}
