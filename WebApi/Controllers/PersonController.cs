using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Models;
using WebApi.Models.Response;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PersonController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Person
        [HttpGet]
        public async Task<ActionResult<Response<IEnumerable<Person>>>> GetPersons()
        {
             var response = new Response<IEnumerable<Person>>();

            try 
            {

                var persons = await _context.Persons.ToListAsync();
                response.Success = 1;
                response.Data = persons;
                response.Message = "OK";
                return Ok(response);
                
            }catch(Exception ex) 
            {
                //se puede agregar un ILogger, Esto te ayuda si algún día necesitas rastrear errores en producción.
                //_logger.LogError(ex, "Error al obtener personas");
                response.Message = ex.Message;
                response.Success = 0;
                return BadRequest(response);
            }
        }

        // GET: api/Person/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<Person>>> GetPerson(int id)
        {
            var response = new Response<Person>();

            try {

                var person = await _context.Persons.FindAsync(id);
                if (person == null)
                {
                    response.Success = 0;
                    response.Message = $"Person with ID : {id} not found";
                    return NotFound(response); //devuelve la respuesta con notFound y ek objeto response 
                }

                response.Success = 1;
                response.Data = person;
                response.Message = "Person found successfully.";
                return Ok(response);
            } catch (Exception ex) {
                
                response.Success = 0;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            
        }

        // PUT: api/Person/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        //public async Task < IActionResult > PutPerson(int id, Person person) //
        //se cambio  IActionResult por ActionResult ya que estoy usando un Response personalizado ,
        //aunque las 2 son validas
        public async Task<ActionResult<Response<Person>>> PutPerson(int id, Person person)

        {
            var response = new Response<Person>();

            if (id != person.Id)
            {
                response.Success = 0;
                response.Message = "ID in path and body do not match.";
                return BadRequest(response);
            }

            _context.Entry(person).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                response.Success = 1;
                response.Data = person;
                response.Message = "Person updated successfully.";
                return Ok(response);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(id))
                {
                    response.Success = 0;
                    response.Message = $"Person with ID : {id} not found";
                    return NotFound(response);
                }
                else
                {
                    throw;
                }
            }

            //return NoContent();
        }

        // POST: api/Person
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Response<Person>>> PostPerson(Person person)
        {
            var response = new Response<Person>();
            try {
                _context.Persons.Add(person);
                await _context.SaveChangesAsync();

                response.Success = 1;
                response.Message = "Person add successfully.";
                response.Data = person;
                return CreatedAtAction("GetPerson", new { id = person.Id }, response);
            }
            catch (Exception ex) {
                response.Success = 0;
                response.Message = ex.Message;
                return BadRequest(response);

            }
        }

        // DELETE: api/Person/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var response = new Response<Person>();

            var person = await _context.Persons.FindAsync(id);
            
            if (person == null)
            {
                response.Success = 0;
                response.Message =  $"Person with ID: {id} not found.";
                return NotFound(response);
            }

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();

            response.Success = 1;
            response.Message = $"Person Id: {id} deleted successfully";
            response.Data = person;
            return Ok(response);
        }

        private bool PersonExists(int id)
        {
            return _context.Persons.Any(e => e.Id == id);
        }
    }
}
