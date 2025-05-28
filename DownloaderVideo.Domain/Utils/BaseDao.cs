using MongoDB.Driver;

namespace DownloaderVideo.Domain.Utils;

public interface BaseDao<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(int page, int pageSize, FilterDefinition<T>? filter = null);

    Task<T> GetIdAsync(string Id);

    Task CreateAsync(T addObject);

    Task RemoveAsync(string Id);

    Task<T> UpdateAsync(string Id, IDictionary<string, object> updateFields);
}
