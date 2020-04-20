using System.Threading.Tasks;
using PlanDoRepeatWeb.Models.Database;

namespace PlanDoRepeatWeb.Implementations.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByLoginAsync(string login);
        Task CreateUserAsync(User user);
    }
}