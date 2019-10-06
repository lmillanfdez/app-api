using System.ComponentModel.DataAnnotations;

public class AddTodoDTO
{
    [StringLength(50, MinimumLength = 5)]
    public string Description { get; set; }
}