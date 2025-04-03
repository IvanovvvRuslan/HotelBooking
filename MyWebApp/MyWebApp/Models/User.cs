using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MyWebApp.Models;

public class User : IdentityUser<int>
{
   public Account Account { get; set; }
}