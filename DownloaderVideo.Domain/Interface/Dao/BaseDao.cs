using MongoDB.Driver;

namespace DownloaderVideo.Infra.Data.Repository.Utils;

/// <summary>
/// Interface for basic data access operations.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface BaseDao<T>
{
    /// <summary>
    /// Retrieves a paginated list of entities.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="filter">The filter definition.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of entities.</returns>
    Task<IEnumerable<T>> GetAllAsync(int page, int pageSize, FilterDefinition<T>? filter = null);

    /// <summary>
    /// Retrieves an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity.</returns>
    Task<T> GetIdAsync(string id);

    /// <summary>
    /// Creates a new entity.
    /// </summary>
    /// <param name="addObject">The entity to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CreateAsync(T addObject);

    /// <summary>
    /// Removes an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to remove.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveAsync(string id);

    /// <summary>
    /// Updates an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to update.</param>
    /// <param name="updateFields">The fields to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated entity.</returns>
    Task<T> UpdateAsync(string id, IDictionary<string, object> updateFields);
}
