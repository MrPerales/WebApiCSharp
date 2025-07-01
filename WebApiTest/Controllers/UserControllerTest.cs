using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;
using WebApi.Models;
using WebApi.Models.Response;
using WebApi.Services;

namespace WebApiTest.Controllers
{
    public class UserControllerTest
    {
        private readonly UsersController _userController;
        //nota : usar Interfaz en service (me daba error al no usar una interfaz)
        private readonly Mock<IUserService> _mockService;


        public UserControllerTest()
        {
            _mockService = new Mock<IUserService>();
            _userController = new UsersController(_mockService.Object);
        }
        /////// GetAllUsers
        [Fact]
        public async Task GetUsers_Ok()
        {
            //arrange
            var mockUsers = new List<User>()
            {
                new User() {
                    Id = 1 ,
                    Nombre="Carlos",
                    Email= "example@mail.com",
                    Password="123"
                },
                new User() {
                    Id = 2 ,
                    Nombre="Carlos2",
                    Email= "example@mail.com",
                    Password="123456"
                },
            };
            //var response = new Response<IEnumerable<User>>() {
            //    Data= mockUsers,
            //    Message= "OK",
            //    Success= true
            //};

            _mockService.Setup(x=>x.GetAllUsers()).ReturnsAsync(mockUsers);

            //act
            var request=await _userController.GetUsers();

            //assert
            var okResult= Assert.IsType<OkObjectResult>(request.Result);
            var okResponse = Assert.IsType<Response<IEnumerable<User>>>(okResult.Value);
            Assert.True(okResponse.Success);
            Assert.Equal("OK", okResponse.Message);
            Assert.Equal(2, okResponse.Data.Count());
        }
        [Fact]
        public async Task GetAllUsers_BadRequest()
        {
            //arrange
            _mockService.Setup(x => x.GetAllUsers()).ThrowsAsync(new Exception("Error"));
            //act
            var request = await _userController.GetUsers();
            //assert
            var badRequest= Assert.IsType<BadRequestObjectResult>(request.Result);
            var result = Assert.IsType<Response<IEnumerable<User>>>(badRequest.Value);
            Assert.False(result.Success);
            Assert.Equal("Error",result.Message);
        }

        ///////////////GetOneUser
        [Fact]
        public async Task GetOneUser_Ok()
        {
            //arrange
            var mockUser= new User() { 
                Id = 1 ,
                Nombre="Carlos",
                Email= "example@mail.com",
                Password="123"
            };
            var response= new Response<User>() { 
                Data = mockUser,
                Success=true,
                Message= "Person found successfully."
            };
            var idUser = 1;

            _mockService.Setup(x=> x.GetOne(idUser)).ReturnsAsync(mockUser);

            //act
            var userResult = await _userController.GetUser(idUser);
            // assert
            //ActionResult<User> envuelve User, por eso necesitas usar result.Result para acceder a OkObjectResult
            var okResult = Assert.IsType<OkObjectResult>(userResult.Result);

            var okResponse = Assert.IsType<Response<User>>(okResult.Value);

            Assert.True(response.Success);
            Assert.Equal("Person found successfully.", response.Message);
            Assert.Equal(idUser, response.Data.Id);
        
        }
        [Fact]
        public async Task GetOneUser_BadRequest()
        {
            //arrange
            var mockUser = new User();
            var idUser = 1;

            _mockService.Setup(x => x.GetOne(idUser)).ThrowsAsync(new Exception("Error"));
            
            //act
            var request = await _userController.GetUser(idUser);

            //assert
            var badRequest= Assert.IsType<BadRequestObjectResult>(request.Result);
            var result = Assert.IsType<Response<User>>(badRequest.Value);

            Assert.False(result.Success);
            Assert.Equal("Error", result.Message);

        }

        [Fact]
        public async Task GetOneUser_NotFound_WhenIdNotFound()
        {
            //arrange
            var idUser = 100001;
            //(User)null => es lo mismo que null as User 
            _mockService.Setup(x => x.GetOne(idUser)).ReturnsAsync((User)null);
            
            //act
            var request= await _userController.GetUser(idUser);

            //assert
            var notFoundRequest= Assert.IsType<NotFoundObjectResult>(request.Result);
            var result = Assert.IsType<Response<User>>(notFoundRequest.Value);
            Assert.False(result.Success);
            Assert.Equal($"Person with ID : {idUser} not found", result.Message);

            
        }

        /////////////PUTUSER
        [Fact]
        public async Task PutUser_Ok_WhenUserUpdtated()
        {
            //arrange
            var idUser = 1;
            var mockUser = new User()
            {
                Id = 1,
                Nombre = "Carlos",
                Email = "example@mail.com",
                Password = "123"
            };
            var newMockUser = new User()
            {
                Id = 1,
                Nombre = "CarlosEdit",
                Email = "example@mail.com",
                Password = "123Edit"
            };

            _mockService.Setup(x => x.UpdateUser(idUser,mockUser)).ReturnsAsync(newMockUser);
            //act
            var request = await _userController.PutUser(idUser, mockUser);

            //assert
            //ActionResult<User> envuelve User, por eso necesitas usar result.Result para acceder a OkObjectResult

            var okResult = Assert.IsType<OkObjectResult>(request.Result);
            var okResponse = Assert.IsType<Response<User>>(okResult.Value);

            Assert.True(okResponse.Success);
            Assert.Equal("CarlosEdit", okResponse.Data.Nombre);

        }

        [Fact]
        public async Task PutUser_NotFoundId()
        {
            //arrange
            var idUser = 1;
            var mockUser = new User();

            _mockService.Setup(x => x.UpdateUser(idUser, mockUser)).ReturnsAsync((User)null);

            //act
            var request = await _userController.PutUser(idUser, mockUser);


            //assert
            var notFoundRequest = Assert.IsType<NotFoundObjectResult>(request.Result);
            var result = Assert.IsType<Response<User>>(notFoundRequest.Value);
            Assert.False(result.Success);
            Assert.Equal($"Person with ID : {idUser} not found", result.Message);
        
        }
    
        ////////////////POSTUSER
        [Fact]
        public async Task PostUser_OK()
        {
            ///arrange
            var mockUser = new User()
            {
                Id = 1,
                Nombre = "Carlos",
                Email = "example@mail.com",
                Password = "123"
            };
            _mockService.Setup(x=>x.AddUser(mockUser)).ReturnsAsync(mockUser);

            //act
            var request = await _userController.PostUser(mockUser);


            //assert
            var okRequest = Assert.IsType<CreatedAtActionResult>(request.Result);
            var result = Assert.IsType<User>(okRequest.Value);
            //ojo usas el okResult 
            Assert.Equal("GetUser", okRequest.ActionName); //<- pon el nombre del metodo al cual llama 
            Assert.Equal(201, okRequest.StatusCode);

        }

        [Fact]
        public async Task PostUser_BadRequest()
        {
            //arrange
            var mockUser = new User();
            _mockService.Setup(x => x.AddUser(mockUser)).ThrowsAsync(new Exception("Error"));
            //act 
            var request= await _userController.PostUser(mockUser);

            var badRequest = Assert.IsType<BadRequestObjectResult>(request.Result);
            var result= Assert.IsType<Response<User>>(badRequest.Value);

            //assert
            Assert.False(result.Success);
            Assert.Equal("Error",result.Message);

        }


        ///////DELETEUSER
        [Fact]
        public async Task DeleteUser_Ok()
        {
            //arrange 
            var idUser = 1;
            var mockUser = new User()
            {
                Id = 1,
                Nombre = "Carlos",
                Email = "example@mail.com",
                Password = "123"
            };
            _mockService.Setup(x => x.DeleteUser(idUser)).ReturnsAsync(mockUser);

            //act 
            
            var request = await _userController.DeleteUser(idUser);

            var okRequest = Assert.IsType<OkObjectResult>(request);
            var result = Assert.IsType<Response<User>>(okRequest.Value);
            //assert 
            Assert.True(result.Success);
            Assert.Equal("Carlos", result.Data.Nombre);
            Assert.Equal($"Person Id: {idUser} deleted successfully", result.Message);


        }

        [Fact]
        public async Task DeleteUser_NotFoundId() { 
        
            var idUser = 1;
            var mockUser = new User();

            _mockService.Setup(x => x.DeleteUser(idUser)).ReturnsAsync((User)null);

            var request =await _userController.DeleteUser(idUser);

            var notFoundRequest= Assert.IsType<NotFoundObjectResult>(request);

            var result = Assert.IsType<Response<User>>(notFoundRequest.Value);

            Assert.False(result.Success);
            Assert.Equal($"Person with ID: {idUser} not found.", result.Message);
    
        }
    }
}
