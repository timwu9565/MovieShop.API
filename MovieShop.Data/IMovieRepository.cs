using MovieShop.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Data
{
    public interface IMovieRepository: IRepository<Movie>
    {
        // async method
        Task<IEnumerable<Movie>> GetHighestGrossingMovies();
    }
}
