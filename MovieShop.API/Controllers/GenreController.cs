using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieShop.Services;

namespace MovieShop.API.Controllers
{
    [Route("api/[controller]")]
    // by default HTTP rounting
    [ApiController]
    public class GenreController : ControllerBase
    //base dont have view
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAllGenres()
        {
            var genres = await _genreService.GetAllGenres();
            if(genres.Any()) //check list have any data
            {
                return Ok(genres);
            }
            return NotFound();
        }

        [HttpGet]
        [Route("{id}/movies")]
        public async Task<IActionResult> GetMoviesByGenre(int id) 
        {
            var movies = await _genreService.GetMoviesByGenre(id);
            return Ok(movies);
        }
    }
}