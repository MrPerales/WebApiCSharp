using WebApi.Models;
using WebApi.Models.Request;
using WebApi.Models.Response;

namespace WebApi.Services
{
    public interface IUserService
    {
        public Task<IEnumerable<User>> GetAllUsers();
        public Task<User> GetOne(int id);
        public Task<User> AddUser(User user);
        public Task<User> UpdateUser(int id, User user);
        public Task<User> DeleteUser(int id);
        public UserResponse Authentication(AuthRequest model);

       
    }
}
