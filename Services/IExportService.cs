using _3_project.Models;

namespace _3_project.Services
{
    /// <summary>
    /// Интерфейс для сервисов экспорта данных.
    /// </summary>
public interface IExportService
{
    Task<string> ExportAsync(IAsyncEnumerable<Person> people, CancellationToken cancellationToken = default);
}

}
