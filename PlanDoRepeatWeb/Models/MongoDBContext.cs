using MongoDB.Driver;
using PlanDoRepeatWeb.Configurations.DatabaseSettings;
using System.Threading.Tasks;

namespace PlanDoRepeatWeb.Models
{
    public class MongoDBContext <Entity>
    {
        private readonly IMongoDatabase database; // база данных
        private readonly string collection;

        public MongoDBContext(IMongoDatabaseSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            database = client.GetDatabase(settings.DatabaseName);
            collection = settings.CollectionName;
        }

        protected IMongoCollection<Entity> Entities
        {
            get { return database.GetCollection<Entity>(collection);  }
        }

        protected Task Add(Entity p)
        {
            return Entities.InsertOneAsync(p);
        }
    }
}
