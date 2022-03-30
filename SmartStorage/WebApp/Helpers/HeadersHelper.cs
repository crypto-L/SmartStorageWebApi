using App.DAL;
using Microsoft.EntityFrameworkCore;
using WebApp.DTO;
namespace WebApp.Helpers;

public class HeadersHelper
{
    
    public static async Task<bool> IsTokenValid(TokenDTO token, AppDbContext context)
    {
        if (token.Token ==  null|| token.UserId == null)
        {
            return false;
        }

        return await context.Users
            .Include(t => t.Token)
            .AnyAsync
            (u => u.Id.ToString() == token.UserId 
                  && u.Token != null
                  && u.Token.TokenString == token.Token);
    }
    
    public static TokenDTO ExtractTokenFromHeaders(IHeaderDictionary headers)
    {
        headers.TryGetValue("userId", out var userId);
        headers.TryGetValue("token", out var tokenString);
        var tokenDto = new TokenDTO(userId, tokenString);
        return tokenDto;
    }
}