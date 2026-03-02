using _3_project.Models;

namespace _3_project.Repositories
{
    /// <summary>
    /// Интерфейс репозитория для работы с Person entity.
    /// </summary>
    public interface IPersonRepository : IRepository<Person, int>
    {
        /// <summary>Поиск с фильтрацией по полям.</summary>
        Task<IReadOnlyList<Person>> SearchAsync(
            string? firstName,
            string? lastName,
            string? city,
            string? country,
            DateTime? date = null,
            CancellationToken cancellationToken = default);
    }
}
