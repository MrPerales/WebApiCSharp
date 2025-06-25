using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Models;
using WebApi.Models.Request;
using WebApi.Models.Response;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _UserSerivice;

        public UsersController(IUserService userService)
        {
            _UserSerivice = userService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<Response<IEnumerable<User>>>> GetUsers()
        {
            var response = new Response<IEnumerable<User>>();
            try {
                var users = await _UserSerivice.GetAllUsers();
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
                var user = await _UserSerivice.GetOne(id);

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

            var updateUser = await _UserSerivice.UpdateUser(id, user);

            if (updateUser == null)
            {
              response.Success = false;
              response.Message = $"Person with ID : {id} not found";
              return NotFound(response);
            }
             
            response.Success = true;
            response.Data = updateUser;
            response.Message = "Person updated successfully.";
            return Ok(response);
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            var response = new Response<User>();
            try {

                var newUser= await _UserSerivice.AddUser(user);
                response.Success = true;
                response.Message = "Person add successfully.";
                response.Data = newUser;
                
                return CreatedAtAction("GetUser", new { id = newUser.Id }, newUser);

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

            var user = await _UserSerivice.DeleteUser(id);
            if (user == null)
            {
                response.Success = false;
                response.Message = $"Person with ID: {id} not found.";
                return NotFound(response);
            }

            response.Success = true;
            response.Message = $"Person Id: {id} deleted successfully";
            response.Data = user;
            return Ok(response);
        }

        //authentication
        [HttpPost("login")]
        public async Task<IActionResult> Authentication([FromBody] AuthRequest model) {

            var response = new Response<UserResponse>();

           var userResponse= _UserSerivice.Authentication(model);

            if (userResponse == null) {
                response.Success = false;
                response.Message = "incorrect user or password";
                return BadRequest(response);
            }

            response.Success = true;
            response.Data = userResponse;

            return Ok(response);
        }
    }
}
