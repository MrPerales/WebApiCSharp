using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Context;
using WebApi.Models;
using WebApi.Models.Request;
using WebApi.Models.Response;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class VentaController : ControllerBase
    {
        private readonly IVentaService _service;
        public VentaController(IVentaService service) {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Add(VentaRequest model) {

            var response = new Response<VentaRequest>();
            //
            try
            {
                Console.WriteLine("entramos al try");
                await _service.Add(model);
                response.Success = true;

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                BadRequest(response);
            }
            return Ok(response);
        
        }
    }
}
