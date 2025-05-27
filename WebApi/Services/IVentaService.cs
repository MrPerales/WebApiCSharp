using WebApi.Models.Request;

namespace WebApi.Services
{
    public interface IVentaService
    {
        public Task Add(VentaRequest model);
    }
}
