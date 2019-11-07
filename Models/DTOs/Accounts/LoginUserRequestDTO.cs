using System.ComponentModel.DataAnnotations;

public class LoginUserRequestDTO
{
    [Required(ErrorMessage = "Email field is mandatory")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password field is mandatory")]
    [MinLength(8, ErrorMessage = "Password field must have at least 8 characters")]
    public string Password { get; set; }
}