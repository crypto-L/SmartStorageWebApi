using App.Domain;

namespace WebApp.DTO;

public class TokenDTO
{
    public string UserId { get; set; }
    public string Token { get; set; }

    public TokenDTO(string userId, string token)
    {
        UserId = userId;
        Token = token;
    }

    public static TokenDTO ConvertEntity(Token tokenEntity)
    {
        var userIdGuid = tokenEntity.UserId ?? tokenEntity.AdminId;
        var userId = userIdGuid.ToString();
        var token = tokenEntity.TokenString;
        return new TokenDTO(userId, token);
    }
}