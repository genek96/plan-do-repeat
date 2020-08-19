using System.Threading.Tasks;
using PlanDoRepeatWeb.Models.Database;

namespace PlanDoRepeatWeb.Implementations.Repositories
{
	public interface IUserRepository
	{
		ValueTask<User> GetUserByLoginAsync(string login);
		Task CreateUserAsync(User user);
	}
}