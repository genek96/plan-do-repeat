﻿using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Commons.StringHelpers;
using PlanDoRepeatWeb.Implementations.Repositories;
using PlanDoRepeatWeb.Models.Authentication;
using PlanDoRepeatWeb.Models.Database;

namespace PlanDoRepeatWeb.Implementations.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUserRepository userRepository;

        public UsersService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<User> LoginAsync(string login, string password)
        {
            var user = await userRepository
                .GetUserByLoginAsync(login)
                .ConfigureAwait(false);

            if (user != null && user.Password == HashPassword(password))
            {
                return user;
            }

            return null;
        }

        public async Task<User> RegisterAsync(RegisterModel registerModel)
        {
            var user = await userRepository
                .GetUserByLoginAsync(registerModel.Email)
                .ConfigureAwait(false);

            if (user != null)
            {
                throw new ArgumentException("Такой пользователь уже существует!");
            }

            await userRepository
                .CreateUserAsync(
                    new User
                    {
                        Login = registerModel.Email,
                        Password = HashPassword(registerModel.Password)
                    })
                .ConfigureAwait(false);

            return await userRepository
                .GetUserByLoginAsync(registerModel.Email)
                .ConfigureAwait(false);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(password.ToByteArray()).ToUtf8String();
        }
    }
}