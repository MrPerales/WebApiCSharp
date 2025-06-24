using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;
using WebApi.Models.Request;
using WebApi.Models.Response;
using WebApi.Services;
namespace WebApiTest.Controllers
{
    public class VentaControllerTest
    {
        private readonly VentaController _ventaController;
        //mockeamos el servicio ya que necesita la base de datos y es MALA practica usar la BD
        //con el maquete Mock de nuGet
        private readonly Mock<IVentaService> _mockService;

        
        public VentaControllerTest()
        {

            _mockService = new Mock<IVentaService>();
            _ventaController = new VentaController(_mockService.Object);
        }

        
        [Fact]
        public async Task Add_OK()
        {
            //arrange
            var venta= new VentaRequest() { 
                IdUser=1,
                Conceptos = new List<ConceptoRequest>() {
                    new ConceptoRequest
                    {
                        Cantidad = 2,
                        IdProducto = 1,
                        PrecioUnitario = 1,
                        Importe = 1,
                    }
                }  
            };
            //simula el llamado del metodo add del servicio 
            _mockService.Setup(x => x.Add(It.IsAny<VentaRequest>())).Returns(Task.CompletedTask);

            //act
            var result= await _ventaController.Add(venta);
            //assert
            //OkObjectResult => se utiliza ya que en el metodo del controler usas Ok(response),
            //si solo pones OK() tienes que usar OKResult 
            var okResult= Assert.IsType<OkObjectResult>(result);

            var response = Assert.IsType<Response<VentaRequest>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Null(response.Message);
        }

        [Fact]
        public async Task Add_BadRequest() {
            //arrange
            var venta = new VentaRequest();

            _mockService.Setup(x => x.Add(It.IsAny<VentaRequest>())).ThrowsAsync(new Exception("Error"));

            //act
            var result = await _ventaController.Add(venta);

            //assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Response<VentaRequest>>(badResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Error",response.Message);


        }
    }
}