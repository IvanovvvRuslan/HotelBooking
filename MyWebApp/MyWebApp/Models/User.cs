﻿using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MyWebApp.Models;

public class User : IdentityUser<int>
{
   public int AccountId { get; set; }
   public Account Account { get; set; }
}