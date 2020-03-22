using System.Threading.Tasks;
using PlanDoRepeatWeb.Models.Authentication;
using PlanDoRepeatWeb.Models.Database;

namespace PlanDoRepeatWeb.Implementations.Services
{
    public interface IUsersService
    {
        Task<bool> LoginAsync(string login, string password);
        Task<User> RegisterAsync(RegisterModel registerModel);
    }
}