using DownloaderVideo.Domain.Entity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace DownloaderVideo.Infra.Data.Context;

[ExcludeFromCodeCoverage]
public abstract class BaseContext<T> where T : IEntity<string>
{
    protected readonly IMongoCollection<T> Collection;

    //Criação do Schema de forma generica
    protected BaseContext(IOptions<DatabaseSettings> options, string collectionName)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase(options.Value.DatabaseName);
        Collection = database.GetCollection<T>(collectionName);
    }

    public async Task<IEnumerable<T>> GetAllAsync(int page, int pageSize, FilterDefinition<T>? filter = null)
    {
        int skip = (page - 1) * pageSize;

        FindOptions<T> options = new()
        {
            Limit = pageSize,
            Skip = skip,
            Sort = Builders<T>.Sort.Descending(x => x.DateCreated) // Ordena por data de criação no próprio banco de dados
        };

        if (filter == null)
        {
            return await Collection.FindSync(_ => true, options).ToListAsync();
        }
        else
            return await Collection.FindSync(filter, options).ToListAsync();
    }

    public async Task CreateAsync(T addObject) //Criação de um objeto generico
    {
        await Collection.InsertOneAsync(addObject);
    }

    public async Task<T> GetIdAsync(string Id) // Busca por Id
    {
        return await Collection.Find(x => x.Id == Id).FirstOrDefaultAsync();
    }

    public async Task RemoveAsync(string Id) // Remoção por Id
    {
        await Collection.DeleteOneAsync(x => x.Id == Id);
    }

    public async Task<T> UpdateAsync(string id, IDictionary<string, object> updateFields) // Atualização de um objeto generico
    {
        var filter = Builders<T>.Filter.Eq(x => x.Id, id);
        var update = Builders<T>.Update.Combine();

        foreach (var field in updateFields)
        {
            //Preciso validar se o valor vier null, náo atualizo ele, criando talvez um if
            if (field.Value != null && !string.IsNullOrEmpty(field.Value.ToString()))
            {
                update = update.Set(field.Key, field.Value);
            }
        }

        var options = new FindOneAndUpdateOptions<T>
        {
            ReturnDocument = ReturnDocument.After
        };

        return await Collection.FindOneAndUpdateAsync(filter, update, options);
    }
}
