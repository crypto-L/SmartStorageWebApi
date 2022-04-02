using Base.Domain;

namespace App.Domain;

public class User : BaseEntity
{
    public string Nickname { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public ICollection<Storage>? UserStorages { get; set; }
    
    public ICollection<User>? UserItems { get; set; }
    public Token? Token { get; set; }
}