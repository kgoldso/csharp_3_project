using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using _3_project.Models;

namespace _3_project.Services
{
    public class XmlExportService
    {
        public string ExportToXml(List<Person> people)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = $"People_{DateTime.Now:yyyyMMdd_HHmmss}.xml";
            string filePath = Path.Combine(desktopPath, fileName);

            var root = new XElement("TestProgram");

            foreach (var person in people)
            {
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

            return filePath;
        }
    }
}
