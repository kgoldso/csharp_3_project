using _3_project.Models;

namespace _3_project.Repositories
{
    /// <summary>
    /// Интерфейс репозитория для работы с Person entity.
    /// </summary>
    public interface IPersonRepository
    {
        /// <summary>
        /// Добавляет записи порциями (batch insert) для оптимизации.
        /// </summary>
        Task AddRangeAsync(IAsyncEnumerable<Person> people, int batchSize = 1000, CancellationToken cancellationToken = default);

        /// <summary>
        /// Поиск с фильтрацией по полям.
        /// </summary>
        Task<List<Person>> SearchAsync(string? firstName, string? lastName, string? city, string? country, DateTime? date = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить общее количество записей.
        /// </summary>
        Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    }
}
