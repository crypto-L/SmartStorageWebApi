using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Admin : BaseEntity
{
    [MaxLength(32)]
    public string Nickname { get; set; }
    
    public string? PasswordHash { get; set; }
}