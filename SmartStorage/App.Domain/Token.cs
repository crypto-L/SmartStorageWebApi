using Base.Domain;

namespace App.Domain;

public class Token : BaseEntity
{
    public string TokenString { get; set; }
    
    public Guid? UserId { get; set; }
    public User? User { get; set; }
    
    public Guid? AdminId { get; set; }
    public Admin? Admin { get; set; }
}