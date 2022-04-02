using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Storage : BaseEntity
{
    [MaxLength(32)] public string StorageName { get; set; } = default!;
    
    public Guid UserId { get; set; }
    public User? User { get; set; }
    
    public ICollection<Item>? Items { get; set; }

    // recursive relationship
    public Guid? ParentStorageId { get; set; }
    public Storage? ParentStorage { get; set; }

    public ICollection<Storage>? SubStorages { get; set; }
}