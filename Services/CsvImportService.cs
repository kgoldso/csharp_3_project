using CsvHelper;
using CsvHelper.Configuration;
using _3_project.Models;
using System.Globalization;
using System.IO;

namespace _3_project.Services
{
    public class CsvImportService
    {
        /// <summary>
        /// Import CSV file and return list of Person objects.
        /// CSV format: Date;FirstName;LastName;SurName;City;Country
        /// </summary>
        public List<Person> ImportFromCsv(string filePath)
        {
            var people = new List<Person>();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) 
            { 
                Delimiter = ";",
                HeaderValidated = null,
                MissingFieldFound = null
            }))
            {
                csv.Context.RegisterClassMap<PersonClassMap>();
                people = csv.GetRecords<Person>().ToList();
            }

            return people;
        }
    }

    /// <summary>
    /// Map CSV columns to Person properties
    /// </summary>
    public sealed class PersonClassMap : ClassMap<Person>
    {
        public PersonClassMap()
        {
            Map(m => m.Date).Index(0).TypeConverter(new DateTimeConverter());
            Map(m => m.FirstName).Index(1);
            Map(m => m.LastName).Index(2);
            Map(m => m.SurName).Index(3);
            Map(m => m.City).Index(4);
            Map(m => m.Country).Index(5);
        }
    }

    /// <summary>
    /// Custom date converter for CSV (format: dd.MM.yyyy)
    /// </summary>
    public class DateTimeConverter : CsvHelper.TypeConversion.DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (DateTime.TryParseExact(text, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }
            throw new Exception($"Invalid date format: {text}. Expected: dd.MM.yyyy");
        }
    }
}
