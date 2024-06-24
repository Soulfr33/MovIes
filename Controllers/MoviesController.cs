using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovIes.Models;
using System.Linq;
using System.Net;

namespace MovIes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MovieContext _context;

        public MoviesController(MovieContext context)
        {
            _context = context;
        }

        // GET All - api/Movies

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            if (_context.Movies == null)
            {
                return NotFound();
            }
            return await _context.Movies.ToListAsync();
        }

        // GET Single - api/Movies/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            if (_context.Movies is null && id == 0)
            {
                return NotFound();
            }
            var movie = await _context.Movies.FindAsync(id);

            if (movie is null)
            {
                return NotFound();
            }

            return movie;
        }

        // POST Single - api/Movies
        [HttpPost("api/movies/add")]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie)
        {

            if (_context.Movies is null)
                return StatusCode(500);
            
            var movies = await _context.Movies.ToListAsync();

            foreach (var item in movies)
            {
                if(item.Title == movie.Title)
                {
                    return StatusCode(400);
                }
            }

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
        }

        // POST Multiple - api/Movie 
        [HttpPost("api/movies/addmovies")]
        public async Task<ActionResult<List<Movie>>> PostMovies([FromBody] List<Movie> movies)
        {
            if (_context.Movies is null)
                return StatusCode(500);

            var existingMovies = await _context.Movies.ToListAsync();

            foreach (var movie in movies)
            {
                foreach (var item in existingMovies)
                {
                    if (item.Title == movie.Title)
                    {
                        return StatusCode(500);
                    }
                    _context.Movies.Add(movie);
                }
            }
            
            await _context.SaveChangesAsync();


            return StatusCode(201);
        }

        // PUT - Update single (api/Movies/5)
        [HttpPut("api/movies/edit/{id}")]
        public async Task<IActionResult> PutMovie(int id, Movie movie)
        {
            if (id != movie.Id)
            {
                return BadRequest();
            }

            _context.Entry(movie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // DELETE Single
        [HttpDelete("api/movies/remove/{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            if (_context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if(movie == null)
            {
                return NotFound();
            }

           _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool MovieExists(long id) 
        {
            return (_context.Movies?.Any(x => x.Id == id)).GetValueOrDefault();
        }

    }
}
