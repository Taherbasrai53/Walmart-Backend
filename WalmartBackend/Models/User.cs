using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalmartBackend.Models
{
    public class User
    {
        [Key] public int UserId { get; set; } 
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }   
    }

    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }    
    }
}
