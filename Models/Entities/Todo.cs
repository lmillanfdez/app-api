using System;
using System.ComponentModel.DataAnnotations;

public class Todo : BaseEntity
{
    [StringLength(50, MinimumLength = 5)]
    public string Description { get; set; }

    public bool Completed { get; set; } = false;

    [Required]
    public DateTime CreatedOn { get; set; }
}