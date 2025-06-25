using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApi.Context;
using WebApi.Models;
using WebApi.Models.Common;
using WebApi.Models.Request;
using WebApi.Models.Response;
using WebApi.Tools;
using System.Security.Claims;

namespace WebApi.Services
{
    public class UserService:IUserService
    {
        private readonly AppDbContext _context;
        private readonly AppSettings _appSettings;
        public UserService(AppDbContext context , IOptions<AppSettings> appSettings) {
            _appSettings = appSettings.Value; //para obtner el secreto 
            _context= context;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetOne(int id)
        {
          return await _context.Users.FindAsync(id);
        }
        public async Task<User> AddUser(User user)
        {
            _context.Users.Add(user);
           await _context.SaveChangesAsync();
           return user;
        }
        public async Task<User> UpdateUser(int id ,User user)
        {
            if (id != user.Id) return null;

            _context.Entry(user).State = EntityState.Modified;

            try { 

                await _context.SaveChangesAsync();
                return user;
            
            } catch (Exception ex) {
                if (!UserExists(id))
                {
                    return null;
                }
                throw; 
            }
        }
        public async Task<User> DeleteUser(int id) {
            var user = await GetOne(id);
            if (user == null)
            {
                return null;
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }
        private bool UserExists(int id)
        {
            return  _context.Users.Any(e => e.Id == id);
        }
    
        //response Authentication
        public UserResponse Authentication(AuthRequest model) {

            var response = new UserResponse();
            
            var spassword = Encrypt.GetSHA256(model.Password); //encriptamos 
            //buscamos al usuario 
            var user = _context.Users.Where(d => d.Email == model.Email &&
                                            d.Password == spassword).FirstOrDefault();
            
            if (user == null) return null;
            
            response.Email = model.Email;
            response.Token = GetToken(user);
            return response;
        }
      
        private string GetToken(User user) {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secreto);

            //info que queremos tener en el token y sus opciones
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {   //lo que vamos a guardar en los claims 
                        new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                        new Claim(ClaimTypes.Email, user.Email)
                    }
                    ),
                Expires = DateTime.UtcNow.AddDays(10), //expira 
                SigningCredentials= new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature) //encriptamos la info 
            };
            //creamos el token 
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return  tokenHandler.WriteToken(token);
        }
    }
}
