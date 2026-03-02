using System.Runtime.CompilerServices;

namespace _3_project.Helpers
{
    /// <summary>
    /// Вспомогательные extension-методы для асинхронных перечислений.
    /// </summary>
    internal static class AsyncEnumerableExtensions
    {
        public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(
            this IEnumerable<T> source,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var item in source)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return item;
            }
        }
    }
}
