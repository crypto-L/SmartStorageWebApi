using Base.Domain;

namespace App.Domain;

public class User : BaseEntity
{
    public string Nickname { get; set; }
    public string PasswordHash { get; set; }
    
    public ICollection<Storage>? UserStorages { get; set; }
}