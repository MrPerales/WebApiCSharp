using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Response
{
    public class UserResponse
    {
        public string Email { get; set; }

        public string Token { get; set; }
    }
}
