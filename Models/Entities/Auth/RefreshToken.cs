using System;
using System.ComponentModel.DataAnnotations;

public class RefreshToken : BaseEntity
{
    public string UserId { get; set; }
    [Required]
    public string RefreshTokenValue { get; set; }
    [Required]
    public DateTime CreatedOnUtc { get; set; }
    [Required]
    public DateTime ExpiresOnUtc { get; set; }
}