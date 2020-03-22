using System.Threading.Tasks;
using System.Security.Cryptography;
using Commons.StringHelpers;
using PlanDoRepeatWeb.Implementations.Repositories;
using PlanDoRepeatWeb.Models.Database;

namespace PlanDoRepeatWeb.Models.Authentication
{
    public class UsersService
    {
        private readonly UserRepository userRepository;

        public UsersService(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<bool> LoginAsync(string login, string password)
        {
            var user = await userRepository
                .GetUserByLoginAsync(login)
                .ConfigureAwait(false);

            return user != null && user.Password == HashPassword(password);
        }

        public async Task<bool> RegisterAsync(string login, string password)
        {
            var user = await userRepository
                    .GetUserByLoginAsync(login)
                    .ConfigureAwait(false);

            if (user == null)
            {
                await userRepository
                    .CreateUserAsync(
                        new User
                        {
                            Login = login,
                            Password = HashPassword(password)
                        })
                    .ConfigureAwait(false);
                return true;
            }
            return false;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(password.ToByteArray()).ToUtf8String();
            }
        }
    }
}
