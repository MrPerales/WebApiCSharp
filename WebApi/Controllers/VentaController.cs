using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Context;
using WebApi.Models;
using WebApi.Models.Request;
using WebApi.Models.Response;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class VentaController : ControllerBase
    {
        private readonly AppDbContext _context;
        public VentaController(AppDbContext context) {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> Add(VentaRequest model) {

            var response = new Response<VentaRequest>(); 

            try
            {
                var venta = new Venta();
                venta.Total = model.Total;
                venta.Fecha = DateTime.Now;
                venta.IdUser = model.IdUser;
                _context.Venta.Add(venta);
               await _context.SaveChangesAsync();

                //ya que son una lista de coceptos leemos cada propiedad y
                //guardamos los datos de los conceptos 
                foreach (var modelConcepto in model.Conceptos) {
                    Console.WriteLine("en los conceptos");
                    var concepto = new Concepto();
                    concepto.Cantidad = modelConcepto.Cantidad;
                    concepto.IdProducto = modelConcepto.IdProducto;
                    concepto.PrecioUnitario= modelConcepto.PrecioUnitario;
                    concepto.Importe = modelConcepto.Importe;
                    concepto.IdVenta = venta.Id;
                    _context.Conceptos.Add(concepto);
                 await _context.SaveChangesAsync();
                }
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
