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
        var correctCredentials = await _context.Users
            .AnyAsync(u => u.Nickname == user.Nickname && u.PasswordHash == user.PasswordHash);
        if (correctCredentials)
        {
            var userEntity = await _context.Users
                .Include(u => u.Token)
                .FirstAsync(u => u.Nickname == user.Nickname && u.PasswordHash == user.PasswordHash);

            if (userEntity.Token != null)
            {
                _context.Tokens.Remove(userEntity.Token);
                await _context.SaveChangesAsync();
            }
            
            //create new token
            var toHash = userEntity.Nickname + userEntity.PasswordHash;
            var token = Hasher.HashString(toHash);
            var tokenEntity = new Token()
            {
                TokenString = token
            };
            userEntity.Token = tokenEntity;
            await _context.SaveChangesAsync();
            return Ok(TokenDTO.ConvertEntity(tokenEntity));
        }
        return BadRequest("Incorrect nickname or password.");
    }
}