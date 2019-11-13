using Microsoft.EntityFrameworkCore;
using MovieShop.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace MovieShop.Data
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(MovieShopDbContext dbContext):base(dbContext)
        {

        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<Purchase>> GetUserPurchasedMovie(int userid)
        {
            var userMovies = await _dbContext.Purchases.Where(p => p.UserId == userid).Include(p=>p.Movie).ToListAsync();
            return userMovies;
        }
    }
}
