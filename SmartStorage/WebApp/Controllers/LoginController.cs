using App.DAL;
using App.Domain;
using Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.DTO;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly AppDbContext _context;

    public LoginController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<TokenDTO>> Post(UserDTO user)
    {
        var userEntity = await _context.Users
            .Include(t => t.Token)
            .FirstOrDefaultAsync(u => u.Nickname == user.Nickname && u.PasswordHash == user.PasswordHash);
        
        if (userEntity == null)
        {
            var admin = await _context.Admins
                .Include(t => t.Token)
                .FirstOrDefaultAsync(a => a.Nickname == user.Nickname && a.PasswordHash == user.PasswordHash);
            if (admin == null)
            {
                return BadRequest("Incorrect nickname or password.");
            }

            if (admin.Token != null)
            {
                _context.Tokens.Remove(admin.Token);
                await _context.SaveChangesAsync();
            }

            var adminToken = Hasher.HashString(admin.Nickname + admin.PasswordHash);
            var adminTokenEntity = new Token()
            {
                TokenString = adminToken
            };
            admin.Token = adminTokenEntity;
            await _context.SaveChangesAsync();
            return Ok(TokenDTO.ConvertEntity(adminTokenEntity));
        }


        if (userEntity.Token != null)
        {
            _context.Tokens.Remove(userEntity.Token);
            await _context.SaveChangesAsync();
        }

        //create new token
        var token = Hasher.HashString(userEntity.Nickname + userEntity.PasswordHash);
        var tokenEntity = new Token()
        {
            TokenString = token
        };
        userEntity.Token = tokenEntity;
        await _context.SaveChangesAsync();
        return Ok(TokenDTO.ConvertEntity(tokenEntity));
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult<bool>> IsAdmin(TokenDTO token)
    {
        var admin = await _context.Admins
            .Include(t => t.Token)
            .FirstOrDefaultAsync();
        
        if (admin == null)
        {
            return false;
        }

        return admin.Id.ToString() == token.UserId
               && admin.Token != null
               && admin.Token.TokenString == token.Token;
    }
}