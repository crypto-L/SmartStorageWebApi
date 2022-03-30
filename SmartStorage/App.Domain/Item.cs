using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Item : BaseEntity
{
    [MaxLength(32)]
    public string Title { get; set; }
    
    [MaxLength(32)]
    public string? SerialNumber { get; set; }
    
    public byte[]? Image { get; set; }
    
    [MaxLength(32)]
    public string? Category { get; set; }
    
    public int? WeightInGrams { get; set; }
    public int? Amount { get; set; }
    
    public Guid UserId { get; set; }
    public User? User { get; set; }
    
    public Guid StorageId { get; set; }
    public Storage? Storage { get; set; }

}