using App.DAL;
using App.Domain;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.DTO;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistrationController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public RegistrationController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpPost]
    public async Task<ActionResult<UserDTO>> Post(UserDTO user)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Nickname == user.Nickname);
        if (userExists)
        {
            return BadRequest($"User with nickname '{user.Nickname}' already exists.");
        }

        var newUserEntity = new User()
        {
            Nickname = user.Nickname,
            PasswordHash = user.PasswordHash
        };
        _context.Users.Add(newUserEntity);
        await _context.SaveChangesAsync();
        var responseUser = UserDTO.ConvertEntity(newUserEntity);
        return Ok(responseUser);
    }

    private string CreateToken()
    {   
        
        return "";
    }
}