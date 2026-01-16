using System.IO;
using System.Xml.Linq;
using _3_project.Models;


namespace _3_project.Services
{
    /// <summary>
    /// Асинхронный сервис экспорта в XML.
    /// </summary>
    public class XmlExportService : IExportService
    {
        public async Task<string> ExportAsync(List<Person> people, CancellationToken cancellationToken = default)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = $"People_{DateTime.Now:yyyyMMdd_HHmmss}.xml";
            string filePath = Path.Combine(desktopPath, fileName);

            await Task.Run(() =>
            {
                var root = new XElement("TestProgram");

                foreach (var person in people)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var record = new XElement("Record",
                        new XAttribute("id", person.Id),
                        new XElement("Date", person.Date.ToString("dd.MM.yyyy")),
                        new XElement("FirstName", person.FirstName),
                        new XElement("LastName", person.LastName),
                        new XElement("SurName", person.SurName),
                        new XElement("City", person.City),
                        new XElement("Country", person.Country)
                    );
                    root.Add(record);
                }

                var document = new XDocument(root);
                document.Save(filePath);

            }, cancellationToken);

            return filePath;
        }
    }
}
