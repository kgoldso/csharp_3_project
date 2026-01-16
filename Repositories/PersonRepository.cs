using Microsoft.EntityFrameworkCore;
using _3_project.Data;
using _3_project.Models;

namespace _3_project.Repositories
{
    /// <summary>
    /// Реализация репозитория для Person с оптимизированной работой с БД.
    /// </summary>
    public class PersonRepository : IPersonRepository
    {
        private readonly AppDbContext _context;

        public PersonRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddRangeAsync(IAsyncEnumerable<Person> people, int batchSize = 1000, CancellationToken cancellationToken = default)
        {
            var batch = new List<Person>(batchSize);

            await foreach (var person in people.WithCancellation(cancellationToken))
            {
                batch.Add(person);

                if (batch.Count >= batchSize)
                {
                    await _context.People.AddRangeAsync(batch, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                    batch.Clear();
                }
            }

            // Сохраняем остаток
            if (batch.Count > 0)
            {
                await _context.People.AddRangeAsync(batch, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<List<Person>> SearchAsync(string? firstName, string? lastName, string? city, string? country, DateTime? date = null, CancellationToken cancellationToken = default)
        {
            var query = _context.People.AsQueryable();

            // Фильтрация на уровне БД (важно для производительности)
            if (!string.IsNullOrWhiteSpace(firstName))
                query = query.Where(p => EF.Functions.Like(p.FirstName, $"%{firstName}%"));

            if (!string.IsNullOrWhiteSpace(lastName))
                query = query.Where(p => EF.Functions.Like(p.LastName, $"%{lastName}%"));

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(p => EF.Functions.Like(p.City, $"%{city}%"));

            if (!string.IsNullOrWhiteSpace(country))
                query = query.Where(p => EF.Functions.Like(p.Country, $"%{country}%"));

            if (date.HasValue)
                query = query.Where(p => p.Date.Date == date.Value.Date);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.People.CountAsync(cancellationToken);
        }
    }
}
