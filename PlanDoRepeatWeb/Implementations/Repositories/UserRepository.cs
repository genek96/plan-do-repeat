using System.Threading.Tasks;
using Commons.MongoDB;
using MongoDB.Driver;
using PlanDoRepeatWeb.Configurations.DatabaseSettings;
using PlanDoRepeatWeb.Models.Database;

namespace PlanDoRepeatWeb.Implementations.Repositories
{
    public class UserRepository : MongoDbContext<User>, IUserRepository
    {
        public UserRepository(UsersDatabaseSettings settings)
            : base(settings)
        {
        }

        public Task<User> GetUserByLoginAsync(string login) => Entities
            .Find(x => x.Login == login)
            .FirstOrDefaultAsync();

        public Task CreateUserAsync(User user) => Entities.InsertOneAsync(user);
    }
}