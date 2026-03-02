using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using _3_project.Models;

namespace _3_project.Services
{
    /// <summary>
    /// Асинхронный потоковый сервис экспорта в XML с поддержкой локализации.
    /// </summary>
    public class XmlExportService : IExportService
    {
        // Локализованные хедеры: ru / en
        private static readonly Dictionary<string, Dictionary<string, string>> _headers = new()
        {
            ["ru"] = new()
            {
                ["Date"]      = "Дата",
                ["FirstName"] = "Имя",
                ["LastName"]  = "Фамилия",
                ["SurName"]   = "Отчество",
                ["City"]      = "Город",
                ["Country"]   = "Страна"
            },
            ["en"] = new()
            {
                ["Date"]      = "Date",
                ["FirstName"] = "FirstName",
                ["LastName"]  = "LastName",
                ["SurName"]   = "SurName",
                ["City"]      = "City",
                ["Country"]   = "Country"
            }
        };

        /// <summary>
        /// Экспортирует записи в XML потоком — без загрузки всех данных в память.
        /// Принимает IAsyncEnumerable, чтобы читать из БД построчно через AsAsyncEnumerable().
        /// </summary>
        public async Task<string> ExportAsync(
            IAsyncEnumerable<Person> people,
            CancellationToken cancellationToken = default)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName    = $"People_{DateTime.Now:yyyyMMdd_HHmmss}.xml";
            string filePath    = Path.Combine(desktopPath, fileName);

            // Выбираем локаль: ru или en (fallback)
            var lang    = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var headers = _headers.GetValueOrDefault(lang, _headers["en"]);

            var settings = new XmlWriterSettings
            {
                Async    = true,
                Indent   = true,
                Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)
            };

            // Потоковая запись: каждая запись пишется сразу на диск, не копится в памяти
            await using var writer = XmlWriter.Create(filePath, settings);

            await writer.WriteStartDocumentAsync();
            await writer.WriteStartElementAsync(null, "TestProgram", null);

            await foreach (var person in people.WithCancellation(cancellationToken))
            {
                await writer.WriteStartElementAsync(null, "Record", null);
                await writer.WriteAttributeStringAsync(null, "id", null, person.Id.ToString());

                await writer.WriteElementStringAsync(null, headers["Date"],      null, person.Date.ToString("dd.MM.yyyy"));
                await writer.WriteElementStringAsync(null, headers["FirstName"], null, person.FirstName);
                await writer.WriteElementStringAsync(null, headers["LastName"],  null, person.LastName);
                await writer.WriteElementStringAsync(null, headers["SurName"],   null, person.SurName);
                await writer.WriteElementStringAsync(null, headers["City"],      null, person.City);
                await writer.WriteElementStringAsync(null, headers["Country"],   null, person.Country);

                await writer.WriteEndElementAsync();
            }

            await writer.WriteEndElementAsync();
            await writer.WriteEndDocumentAsync();

            return filePath;
        }
    }
}
