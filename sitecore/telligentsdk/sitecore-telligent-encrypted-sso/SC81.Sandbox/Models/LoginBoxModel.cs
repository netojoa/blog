using System.ComponentModel.DataAnnotations;

namespace SC81.Sandbox.Models
{
    public class LoginBoxModel
    {

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
        
    }
}