using System;
using System.ComponentModel.DataAnnotations;

namespace App_api.Models
{
    public class Todo
    {
        public int Id { get; set; }
        [StringLength(50, MinimumLength = 5)]
        public string Description { get; set; }
        [Required]
        public bool Completed { get; set; }
    }
}