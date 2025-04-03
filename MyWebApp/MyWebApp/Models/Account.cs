using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models;

public class Account
{
    [Key]
    public int Id { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public string? Description { get; set; }

    public int UserId { get; set; }

    public User User { get; set; }
}