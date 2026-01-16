using System.IO;
using System.Runtime.CompilerServices;
using CsvHelper;
using CsvHelper.Configuration;
using _3_project.Models;
using System.Globalization;


namespace _3_project.Services
{
    /// <summary>
    /// Оптимизированный сервис импорта CSV с потоковой обработкой.
    /// Не загружает весь файл в память.
    /// </summary>
    public class CsvImportService
    {
        /// <summary>
        /// Импортирует CSV файл потоково через IAsyncEnumerable.
        /// Подходит для файлов 1млн+ строк.
        /// </summary>
        public async IAsyncEnumerable<Person> ImportFromCsvAsync(
    string filePath, 
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
{
    using var reader = new StreamReader(filePath);
    
    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        Delimiter = ";",
        HeaderValidated = null,
        MissingFieldFound = null,
        BadDataFound = null
    };
    
    using var csv = new CsvReader(reader, config);
    csv.Context.RegisterClassMap<PersonClassMap>();

    // Асинхронное чтение записей по одной
    var records = csv.GetRecordsAsync<Person>(cancellationToken);

    await foreach (var person in records.WithCancellation(cancellationToken))
    {
        yield return person;
    }
}


        /// <summary>
        /// Подсчёт строк в файле без полной загрузки.
        /// </summary>
        public async Task<int> GetRecordCountAsync(string filePath, CancellationToken cancellationToken = default)
        {
            int count = 0;
            using var reader = new StreamReader(filePath);
            
            // Пропускаем заголовок
            await reader.ReadLineAsync();
            
            while (await reader.ReadLineAsync() != null)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                count++;
            }

            return count;
        }

        /// <summary>
        /// Map CSV columns to Person properties
        /// </summary>
        private sealed class PersonClassMap : ClassMap<Person>
        {
            public PersonClassMap()
            {
                Map(m => m.Date).Index(0).TypeConverter<DateTimeConverter>();
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
        private class DateTimeConverter : CsvHelper.TypeConversion.DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                if (DateTime.TryParseExact(text, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                {
                    return result;
                }

                // Возвращаем дефолтную дату вместо исключения
                return DateTime.MinValue;
            }
        }
    }
}
