using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using WebApi.Context;
using WebApi.Models;
using WebApi.Models.Common;
using WebApi.Services;

namespace WebApiTest.Services
{
    public class UserServiceTest
    {
        // NOTA => para hacer test del servicio tenermos que usar la base de datos EN MEMORIA
        //para eso agregar el paquete InMemory , NO usar la real
        [Fact]
        public async Task AddUser_ShouldAddUserToDataBase()
        {
            //arrange
            
            var options = new DbContextOptionsBuilder<AppDbContext>()
                            .UseInMemoryDatabase(databaseName: "dataBase")
                            .Options;
            // Usamos el contexto para simular la BD
            using var context = new AppDbContext(options);

            //simulamos el IOptions<AppSettings> appSettings
            var mockSettings = Options.Create(new AppSettings
            {
                Secreto = "super-secret-key"
            });
            //iniciamos servicio
            var service = new UserService(context,mockSettings);

            var mockUser = new User()
            {
                Id = 1,
                Nombre = "Carlos",
                Email = "example@mail.com",
                Password = "123"
            };

            //act
            var request = await service.AddUser(mockUser);

            //assert
            Assert.NotNull(request);
            Assert.Equal("Carlos", request.Nombre);
        }
        ////////////GetAllUsers
        [Fact]
        public async Task GetAllUsers_ShouldReturnUsersList_FromDataBase()
        {
            //arrange

            var options = new DbContextOptionsBuilder<AppDbContext>()
                            .UseInMemoryDatabase(databaseName: "dataBase")
                            .Options;
            // Usamos el contexto para simular la BD
            using var context = new AppDbContext(options);

            //simulamos el IOptions<AppSettings> appSettings
            var mockSettings = Options.Create(new AppSettings
            {
                Secreto = "super-secret-key"
            });
            //iniciamos servicio
            var service = new UserService(context, mockSettings);

            var user1 = new User()
            {
                Id = 1,
                Nombre = "Carlos",
                Email = "example@mail.com",
                Password = "123"
            };
            var user2 = new User()
            {
                Id = 2,
                Nombre = "Carlos2",
                Email = "example2@mail.com",
                Password = "123456"
            };
            
            // agregamos los datos a la database en menoria 
            context.Users.Add(user1);
            context.Users.Add(user2);
            await context.SaveChangesAsync();
            //act

            var request = await service.GetAllUsers();
            
            //asserts
            Assert.NotNull(request);
            Assert.Equal(2, request.Count());
            Assert.Contains(request, x => x.Nombre == "Carlos");
            Assert.Contains(request, x => x.Password == "123456");

        }
        
        
        //UpdateUser
        [Fact]
        public async Task UpdateUser_ShouldUpdateUserAndReturnAUser()
        {
            //arrange

            var options = new DbContextOptionsBuilder<AppDbContext>()
                            .UseInMemoryDatabase(databaseName: "dataBase")
                            .Options;
            // Usamos el contexto para simular la BD
            using var context = new AppDbContext(options);

            //simulamos el IOptions<AppSettings> appSettings
            var mockSettings = Options.Create(new AppSettings
            {
                Secreto = "super-secret-key"
            });
            //iniciamos servicio
            var service = new UserService(context, mockSettings);

            var mockUser = new User()
            {
                Id = 1,
                Nombre = "Carlos",
                Email = "example@mail.com",
                Password = "123"
            };
          

            var idUser = 1;
            // agregamos los datos a la database en menoria 
            context.Users.Add(mockUser);
            await context.SaveChangesAsync(); //guardamos los cambios

            //editamos de esta manera ya que si agregamos otro usario agregariamos otra instancia con el mismo id
            //y eso daria error ya que estamos usando 
            //Entity().state..... en el metodo del servicio  
            mockUser.Nombre = "Carlos Editado";
            mockUser.Email = "editado@mail.com";
            //act
            var request= await service.UpdateUser(idUser, mockUser);

            //assert
            Assert.NotNull(request);
            Assert.Equal("Carlos Editado", request.Nombre);
            Assert.Equal("editado@mail.com", request.Email);

            // Verificamos que también se guardó en la BD correctamente
            var userInDb = await context.Users.FindAsync(idUser);
            Assert.NotNull(userInDb);
            Assert.Equal("Carlos Editado", userInDb.Nombre);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnNull_WhenIdIsNotMatch()
        {
            //arrange

            var options = new DbContextOptionsBuilder<AppDbContext>()
                            .UseInMemoryDatabase(databaseName: "dataBase")
                            .Options;
            // Usamos el contexto para simular la BD
            using var context = new AppDbContext(options);

            //simulamos el IOptions<AppSettings> appSettings
            var mockSettings = Options.Create(new AppSettings
            {
                Secreto = "super-secret-key"
            });
            //iniciamos servicio
            var service = new UserService(context, mockSettings);

            var mockUser = new User()
            {
                Id = 1,
                Nombre = "Carlos",
                Email = "example@mail.com",
                Password = "123"
            };


            var idUser = 100;
            // agregamos los datos a la database en menoria 
            context.Users.Add(mockUser);
            await context.SaveChangesAsync(); //guardamos los cambios

            //act

            var request= await service.UpdateUser(idUser,mockUser);
            
            //assert
            
            Assert.Null(request);
        }
        ///////////////////////// Error al ejecutar el test /////////////////////
        //[Fact]
        //public async Task UpdateUser_ShouldThrowException_WhenSaveFails()
        //{
        //    //en este test se va a utilizar mock , 
        //    //ya que es para simular que saveChangesAsync falle 

        //    //arrange

        //    var options = new DbContextOptionsBuilder<AppDbContext>()
        //                    .UseInMemoryDatabase(databaseName: "dataBase")
        //                    .Options;
        //    // Usamos el contexto para simular la BD
        //    var mockContext = new Mock<AppDbContext>();

        //    //simulamos el IOptions<AppSettings> appSettings
        //    var mockSettings = Options.Create(new AppSettings
        //    {
        //        Secreto = "super-secret-key"
        //    });

        //    var mockUser = new User() {
        //        Id = 1,
        //        Nombre = "Carlos",
        //        Email = "example@mail.com",
        //        Password = "123456"
        //    };


        //    var idUser = 100;
        //    //// agregamos los datos a la database en menoria 
        //    //mockContext.Object.Users.Add(mockUser);
        //    //await mockContext.Object.SaveChangesAsync(); //guardamos los cambios

        //    mockContext.Setup(c => c.SaveChangesAsync(default))
        //      .ThrowsAsync(new Exception("DB error"));

        //    mockContext.Setup(c => c.Entry(It.IsAny<User>()))
        //   .Returns(EntityEntryFake(mockUser));

        //    //iniciamos servicio
        //    var service = new UserService(mockContext.Object, mockSettings);

        //    //act
        //    var exception =  await Assert.ThrowsAsync<DbUpdateException>(() => service.UpdateUser(idUser,mockUser));
        //    //var request = await service.UpdateUser(idUser, mockUser);

        //    //assert

        //    Assert.Equal("DB error", exception.Message);
        //}

        //private static EntityEntry<User> EntityEntryFake(User user)
        //{
        //    var mockEntry = new Mock<EntityEntry<User>>();
        //    mockEntry.SetupAllProperties(); // permite usar .State
        //    return mockEntry.Object;
        //}

        [Fact]
        public async Task DeleteUser_UserDeletedSuccessfully()
        {
            //arrange

            var options = new DbContextOptionsBuilder<AppDbContext>()
                            .UseInMemoryDatabase(databaseName: "dataBase")
                            .Options;
            // Usamos el contexto para simular la BD
            using var context = new AppDbContext(options);

            //simulamos el IOptions<AppSettings> appSettings
            var mockSettings = Options.Create(new AppSettings
            {
                Secreto = "super-secret-key"
            });
            //iniciamos servicio
            var service = new UserService(context, mockSettings);

            var mockUser = new User()
            {
                Id = 1,
                Nombre = "Carlos",
                Email = "example@mail.com",
                Password = "123"
            };
            var mockUser2 = new User()
            {
                Id = 2,
                Nombre = "Carlos2",
                Email = "example@mail.com",
                Password = "123"
            };


            var idUser = 1;
            // agregamos los datos a la database en menoria 
            context.Users.Add(mockUser);
            context.Users.Add(mockUser2);
            await context.SaveChangesAsync(); //guardamos los cambios


            //act
            var request = await service.DeleteUser(idUser);
            var allUsers= await service.GetAllUsers();

            //assert

            // Verificamos que se elemino en la BD correctamente
            var userInDb = await context.Users.FindAsync(idUser);
            Assert.Null(userInDb);

            //verificamos que  solo quede un usuario 
            Assert.Single(allUsers);//para saber si hay una coleccion  users.Count == 1
        }
        [Fact]
        public async Task DeleteUser_ReturnNull_WhenIdIsNotMatch()
        {
            //arrange

            var options = new DbContextOptionsBuilder<AppDbContext>()
                            .UseInMemoryDatabase(databaseName: "dataBase")
                            .Options;
            // Usamos el contexto para simular la BD
            using var context = new AppDbContext(options);

            //simulamos el IOptions<AppSettings> appSettings
            var mockSettings = Options.Create(new AppSettings
            {
                Secreto = "super-secret-key"
            });
            //iniciamos servicio
            var service = new UserService(context, mockSettings);

            var mockUser = new User()
            {
                Id = 1,
                Nombre = "Carlos",
                Email = "example@mail.com",
                Password = "123"
            };

            var idUser = 100;
            // agregamos los datos a la database en menoria 
            context.Users.Add(mockUser);
            await context.SaveChangesAsync(); //guardamos los cambios


            //act
            var request = await service.DeleteUser(idUser);

            //assert
            Assert.Null(request);

        }

    }
}
