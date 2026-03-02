using System.Drawing;
using System.Globalization;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using _3_project.Models;

namespace _3_project.Services
{
    /// <summary>
    /// Сервис экспорта в Excel через EPPlus 7.
    /// </summary>
    public class ExcelExportService : IExportService
    {
        // Локализованные хедеры: ru / en
        private static readonly Dictionary<string, string[]> _headers = new()
        {
            ["ru"] = ["ID", "Дата", "Имя", "Фамилия", "Отчество", "Город", "Страна"],
            ["en"] = ["ID", "Date", "First Name", "Last Name", "SurName", "City", "Country"]
        };

        /// <summary>
        /// Материализует IAsyncEnumerable в список и экспортирует в .xlsx.
        /// EPPlus требует случайный доступ к строкам, потоковая запись невозможна.
        /// </summary>
        public async Task<string> ExportAsync(
            IAsyncEnumerable<Person> people,
            CancellationToken cancellationToken = default)
        {
            // EPPlus 7 — установка лицензии
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Материализуем стрим в список (EPPlus не поддерживает построчный стриминг)
            var list = new List<Person>();
            await foreach (var person in people.WithCancellation(cancellationToken))
                list.Add(person);

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName    = $"People_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string filePath    = Path.Combine(desktopPath, fileName);

            // Выбираем локаль: ru или en (fallback)
            var lang    = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var headers = _headers.GetValueOrDefault(lang, _headers["en"]);

            await Task.Run(() =>
            {
                using var package = new ExcelPackage();
                var worksheet     = package.Workbook.Worksheets.Add("People");

                // Заголовки с локализацией
                for (int col = 0; col < headers.Length; col++)
                    worksheet.Cells[1, col + 1].Value = headers[col];

                // Стиль заголовка
                using (var range = worksheet.Cells[1, 1, 1, headers.Length])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }

                // Данные
                for (int i = 0; i < list.Count; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var person = list[i];
                    int row    = i + 2;

                    worksheet.Cells[row, 1].Value = person.Id;
                    worksheet.Cells[row, 2].Value = person.Date.ToString("dd.MM.yyyy");
                    worksheet.Cells[row, 3].Value = person.FirstName;
                    worksheet.Cells[row, 4].Value = person.LastName;
                    worksheet.Cells[row, 5].Value = person.SurName;
                    worksheet.Cells[row, 6].Value = person.City;
                    worksheet.Cells[row, 7].Value = person.Country;
                }

                // Auto-fit колонок
                worksheet.Cells.AutoFitColumns();

                // Сохранение
                package.SaveAs(new FileInfo(filePath));

            }, cancellationToken);

            return filePath;
        }
    }
}
