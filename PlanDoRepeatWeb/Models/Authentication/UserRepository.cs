using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using PlanDoRepeatWeb.Configurations.DatabaseSettings;
using System.Threading.Tasks;

namespace PlanDoRepeatWeb.Models.Authentication
{
    public class UserRepository : MongoDBContext<User>
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
