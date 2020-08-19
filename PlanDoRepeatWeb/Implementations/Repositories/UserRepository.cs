using System.Linq;
using System.Threading.Tasks;
using Commons.MongoDB;
using PlanDoRepeatWeb.Configurations.DatabaseSettings;
using PlanDoRepeatWeb.Models.Database;

namespace PlanDoRepeatWeb.Implementations.Repositories
{
	public class UserRepository : MongoDbRepository<User>, IUserRepository
	{
		public UserRepository(UsersDatabaseSettings settings)
			: base(settings)
		{
		}

		public ValueTask<User> GetUserByLoginAsync(string login)
		{
			return FindAsync(u => u.Login == login).FirstOrDefaultAsync();
		}

		public Task CreateUserAsync(User user) => AddAsync(user);
	}
}