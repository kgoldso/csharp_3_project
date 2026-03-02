namespace _3_project.Repositories
{
    /// <summary>
    /// Базовый generic-интерфейс репозитория (паттерн Repository).
    /// </summary>
    public interface IRepository<T, TKey> where T : class
    {
        /// <summary>Получить запись по первичному ключу.</summary>
        Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

        /// <summary>Получить общее количество записей.</summary>
        Task<int> GetCountAsync(CancellationToken cancellationToken = default);

        /// <summary>Добавить записи порциями (batch insert) для оптимизации.</summary>
        Task AddRangeAsync(
            IAsyncEnumerable<T> entities,
            int batchSize = 1000,
            CancellationToken cancellationToken = default);
    }
}
