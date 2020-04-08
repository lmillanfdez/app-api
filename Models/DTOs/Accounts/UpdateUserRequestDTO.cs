using System.ComponentModel.DataAnnotations;

public class UpdateUserRequestDTO
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    
    [Required]
    public string FirstName { get; set; }
    public string LastName { get; set; }
}