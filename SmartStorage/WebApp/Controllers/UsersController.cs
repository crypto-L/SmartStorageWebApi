using App.DAL;
using App.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.DTO;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public UsersController(AppDbContext context)
    {
        _context = context;
    }
    
    //GET api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> Get()
    {
        var users = await _context.Users
            .Include(u => u.UserStorages)
            .ToListAsync();
        
        var usersDtos = new List<UserDTO>();
        foreach (var user in users)
        {
            var userDto = UserDTO.ConvertEntity(user);
            usersDtos.Add(userDto);
        }
        return usersDtos;
    }

    //GET api/users/"guid-string"
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> Get(string id)
    {
        var user = await _context.Users
            .Where(u => u.Id.ToString() == id)
            .Include(s => s.UserStorages)
                .Where(t => t.UserStorages != null)
            .FirstOrDefaultAsync();
        
        if (user == null)
        {
            return NotFound($"User with id {id} does not exist.");
        }
        // await _context.Entry(user)
        //     .Collection(u => u.UserStorages)
        //     .Query()
        //         .FirstOrDefaultAsync();
        return Ok(UserDTO.ConvertEntity(user));
    }

    [HttpPost]
    public async Task<ActionResult<UserDTO>> Post(UserDTO user)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Nickname == user.Nickname);
        if (userExists)
        {
            return BadRequest("User already exist!");
        }

        var userEntity = new User()
        {
            Nickname = user.Nickname,
            PasswordHash = user.PasswordHash
        };
        await _context.Users.AddAsync(userEntity);
        await _context.SaveChangesAsync();
        
        return Ok(UserDTO.ConvertEntity(userEntity));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<UserDTO>> Delete(string id)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id.ToString() == id);
        if (userExists)
        {
            var user = await _context.Users.FirstAsync(u => u.Id.ToString() == id);
            var userDto = UserDTO.ConvertEntity(user);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(userDto);
        }
        return BadRequest($"User with id {id} does not exist.");
    }

    //NB! updates only nickname/password
    [HttpPut("{id}")]
    public async Task<ActionResult<UserDTO>> Put(string id, UserDTO user)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id.ToString() == id);
        if (userExists)
        {
            var userEntity = await _context.Users.FirstAsync(u => u.Id.ToString() == id);
            userEntity.Nickname = user.Nickname;
            userEntity.PasswordHash = user.PasswordHash;
            await _context.SaveChangesAsync();
            var userDto = UserDTO.ConvertEntity(userEntity);
            return Ok(userDto);
        }
        return BadRequest("Something goes wrong...");
    }

}