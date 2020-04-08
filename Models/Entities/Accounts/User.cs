using System;
using System.ComponentModel.DataAnnotations;

public class User : BaseEntity
{
    public string Guid { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    public string EmailAddress { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public DateTime CreatedOnUtc { get; set; }
    public bool Active { get; set; }
    public bool Deleted { get; set; }
}