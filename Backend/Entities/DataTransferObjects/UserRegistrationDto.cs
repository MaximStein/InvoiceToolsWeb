using System.ComponentModel.DataAnnotations;
namespace Backend.Entities
{
    public class UserRegistrationDto
    {
       
        [Required]
       // [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
