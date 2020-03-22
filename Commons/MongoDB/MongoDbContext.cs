using System.Threading.Tasks;
using MongoDB.Driver;

namespace Commons.MongoDB
{
    public class MongoDbContext <TEntity>
    {
        private readonly IMongoDatabase database; // база данных
        private readonly string collection;

        public MongoDbContext(IMongoDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            database = client.GetDatabase(settings.DatabaseName);
            collection = settings.CollectionName;
        }

        protected IMongoCollection<TEntity> Entities => database.GetCollection<TEntity>(collection);

        protected Task Add(TEntity p)
        {
            return Entities.InsertOneAsync(p);
        }
    }
}
