using System.IO;
using OfficeOpenXml;
using _3_project.Models;

namespace _3_project.Services
{
    /// <summary>
    /// Сервис экспорта в Excel через EPPlus 7.
    /// </summary>
    public class ExcelExportService : IExportService
    {
        public async Task<string> ExportAsync(List<Person> people, CancellationToken cancellationToken = default)
        {
            // EPPlus 7 - простая установка лицензии
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = $"People_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string filePath = Path.Combine(desktopPath, fileName);

            await Task.Run(() =>
            {
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("People");

                // Заголовки
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Date";
                worksheet.Cells[1, 3].Value = "First Name";
                worksheet.Cells[1, 4].Value = "Last Name";
                worksheet.Cells[1, 5].Value = "SurName";
                worksheet.Cells[1, 6].Value = "City";
                worksheet.Cells[1, 7].Value = "Country";

                // Стиль заголовка
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Данные
                for (int i = 0; i < people.Count; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var person = people[i];
                    int row = i + 2;

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
