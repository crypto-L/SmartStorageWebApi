using App.Domain;

namespace WebApp.DTO;

public class AdminDTO
{
    public string Id { get; set; }
    public string Nickname { get; set; }
    public string? PasswordHash { get; set; }

    public AdminDTO(Guid id, string nickname, string? passwordHash)
    {
        Id = id.ToString();
        Nickname = nickname;
        PasswordHash = passwordHash;
    }

    public static AdminDTO ConvertEntity(Admin entity)
    {
        var id = entity.Id;
        var nickname = entity.Nickname;
        var passwordHash = entity.PasswordHash;
        return new AdminDTO(id, nickname, passwordHash);
    }

}