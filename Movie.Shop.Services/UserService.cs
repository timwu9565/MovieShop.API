using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MovieShop.Data;
using MovieShop.Entities;

namespace MovieShop.Services
{
    public class UserService : IUserService
    {
        // dependency injection
        private readonly IUserRepository _userRepository;
        private readonly ICryptoService _cryptoService;
        public UserService(IUserRepository userRepository, ICryptoService cryptoService)
        {
            _userRepository = userRepository;
            _cryptoService = cryptoService;
        }
        public async Task<User> CreateUser(string email, string password, string firstName, string lastName)
        {
            //check email is unqiue
            var dbUser = await _userRepository.GetUserByEmail(email);
            if (dbUser != null)
            {
                return null;
            }
            var salt = _cryptoService.CreateSalt();
            var hashPassword = _cryptoService.HashPassword(password, salt);
            var user = new User
            {
                Email = email, 
                FirstName = firstName, 
                LastName = lastName, 
                HashedPassword = hashPassword,
                Salt = salt
            };
            var CreatedUser = await _userRepository.AddAsync(user);
            return CreatedUser;
        }

        public async Task<IEnumerable<Purchase>> GetPurchases(int userid)
        {
            return await _userRepository.GetUserPurchasedMovie(userid);
        }

        public async Task<User> ValidateUser(string email, string password)
        {
            var dbUser = await _userRepository.GetUserByEmail(email);
            if (dbUser == null)
            {
                return null;
            }
            var salt = dbUser.Salt;
            var hashPassword = _cryptoService.HashPassword(password, salt);
            var real_hashPassword = dbUser.HashedPassword;
            if (hashPassword != real_hashPassword)
            {
                return null;
            }
            else return dbUser;

        }
    }
}
