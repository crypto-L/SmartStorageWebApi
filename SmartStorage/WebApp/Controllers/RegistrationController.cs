using App.DAL;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> Post(UserDTO user)
    {
        Console.WriteLine(user.Nickname);
        Console.WriteLine(user.PasswordHash);
        return Ok(user);
    }
}