using System.Windows;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;
using _3_project.Data;
using _3_project.Models;
using _3_project.Services;

namespace _3_project
{
    public partial class MainWindow : Window
    {
        private CsvImportService _importService;
        private AppDbContext _dbContext;

        public MainWindow()
        {
            InitializeComponent();
            _importService = new CsvImportService();
            _dbContext = new AppDbContext();
        }

        /// <summary>
        /// Handle Import Button Click
        /// </summary>
        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            // Open file dialog
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                Title = "Select CSV file to import"
            };

            if (openFileDialog.ShowDialog() != true)
                return; // User cancelled

            try
            {
                // Import from CSV
                var people = _importService.ImportFromCsv(openFileDialog.FileName);

                if (people.Count == 0)
                {
                    ImportStatusText.Text = "❌ No data found in CSV";
                    return;
                }

                // Save to database
                _dbContext.People.AddRange(people);
                _dbContext.SaveChanges();

                ImportStatusText.Text = $"✓ Imported {people.Count} records";
            }
            catch (Exception ex)
            {
                ImportStatusText.Text = $"❌ Error: {ex.Message}";
            }
        }

        /// <summary>
        /// Handle Search Button Click
        /// </summary>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var firstName = FirstNameFilter.Text?.Trim() ?? "";
                var lastName = LastNameFilter.Text?.Trim() ?? "";
                var city = CityFilter.Text?.Trim() ?? "";
                var country = CountryFilter.Text?.Trim() ?? "";

                // LINQ query with filters
                var query = _dbContext.People.AsEnumerable();

                if (!string.IsNullOrEmpty(firstName))
                    query = query.Where(p => p.FirstName.Contains(firstName, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(lastName))
                    query = query.Where(p => p.LastName.Contains(lastName, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(city))
                    query = query.Where(p => p.City.Contains(city, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(country))
                    query = query.Where(p => p.Country.Contains(country, StringComparison.OrdinalIgnoreCase));

                var results = query.ToList();

                // Display results
                ResultsDataGrid.ItemsSource = results;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Search error: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle Export to Excel Button Click
        /// </summary>
        /// <summary>
/// Handle Export to Excel Button Click
/// </summary>
private void ExportExcelButton_Click(object sender, RoutedEventArgs e)
{
    try
    {
        var results = ResultsDataGrid.ItemsSource as List<Person>;
        if (results == null || results.Count == 0)
        {
            MessageBox.Show("No data to export");
            return;
        }

        // For now, export as CSV (simpler than Excel)
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string fileName = $"People_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        string filePath = Path.Combine(desktopPath, fileName);

        using (var writer = new StreamWriter(filePath))
        {
            // Headers
            writer.WriteLine("Date;FirstName;LastName;SurName;City;Country");

            // Data
            foreach (var person in results)
            {
                writer.WriteLine($"{person.Date:dd.MM.yyyy};{person.FirstName};{person.LastName};{person.SurName};{person.City};{person.Country}");
            }
        }

        MessageBox.Show($"Exported to: {filePath}");
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Export error: {ex.Message}");
    }
}

        /// <summary>
        /// Handle Export to XML Button Click
        /// </summary>
        private void ExportXmlButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var results = ResultsDataGrid.ItemsSource as List<Person>;
                if (results == null || results.Count == 0)
                {
                    MessageBox.Show("No data to export");
                    return;
                }

                var xmlService = new XmlExportService();
                var filePath = xmlService.ExportToXml(results);
                MessageBox.Show($"Exported to: {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export error: {ex.Message}");
            }
        }
    }
}
