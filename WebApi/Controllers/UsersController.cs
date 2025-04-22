using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Models;
using WebApi.Models.Response;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<Response<IEnumerable<User>>>> GetUsers()
        {
            var response = new Response<IEnumerable<User>>();
            try {
                var users = await _context.Users.ToListAsync();
                response.Success = true;
                response.Data = users;
                response.Message = "OK";
                return Ok(response);
            } catch (Exception ex) {
                //se puede agregar un ILogger, Esto te ayuda si algún día necesitas rastrear errores en producción.
                //_logger.LogError(ex, "Error al obtener personas");
                response.Message = ex.Message;
                response.Success = false;
                return BadRequest(response);
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var response = new Response<User>();
            try { 
                 var user = await _context.Users.FindAsync(id);

                if (user == null)
                {   
                    response.Success = false;
                    response.Message= $"Person with ID : {id} not found";
                    return NotFound(response);
                }
                response.Success = true;
                response.Data = user;
                response.Message = "Person found successfully.";
                return Ok(response);
            
            } catch (Exception ex) {

                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }

        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<Response<User>>> PutUser(int id, User user)
        {
            var response = new Response<User>();

            if (id != user.Id)
            {
                response.Success = false;
                response.Message = "ID in path and body do not match.";
                return BadRequest(response);
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                response.Success = true;
                response.Data = user;
                response.Message = "Person updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    response.Success = false;
                    response.Message = $"Person with ID : {id} not found";
                    return NotFound(response);
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            var response = new Response<User>();
            try {

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Person add successfully.";
                response.Data = user;
                
                return CreatedAtAction("GetUser", new { id = user.Id }, user);

            }
            catch (Exception ex) {

                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);


            }
            
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var response = new Response<User>();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                response.Success = false;
                response.Message = $"Person with ID: {id} not found.";
                return NotFound(response);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = $"Person Id: {id} deleted successfully";
            response.Data = user;
            return Ok(response);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
