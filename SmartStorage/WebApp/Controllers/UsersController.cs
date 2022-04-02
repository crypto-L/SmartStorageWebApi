using App.DAL;
using App.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.DTO;
using WebApp.Helpers;

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
        var token = HeadersHelper.ExtractTokenFromHeaders(Request.Headers);
        var isTokenValid = await HeadersHelper.IsAdminTokenValid(token, _context);
        if (!isTokenValid) return BadRequest("You have no rights.");

        var users = await _context.Users.ToListAsync();

        return users.Select(UserDTO.ConvertEntityWithoutPassword).ToList();
    }

    //GET api/users/"guid-string"
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> Get(string id)
    {
        var token = HeadersHelper.ExtractTokenFromHeaders(Request.Headers);
        var isTokenValid = await HeadersHelper.IsAdminTokenValid(token, _context);
        if (!isTokenValid) return BadRequest("You have no rights.");
        
        var user = await _context.Users
            .Where(u => u.Id.ToString() == id)
            .FirstOrDefaultAsync();
        
        if (user == null)
        {
            return NotFound($"User with id {id} does not exist.");
        }
        return Ok(UserDTO.ConvertEntityWithoutPassword(user));
    }

    [HttpGet]
    [Route("[action]/{id}")]
    public async Task<ActionResult<int>> GetUserItemsCount(string id)
    {
        var token = HeadersHelper.ExtractTokenFromHeaders(Request.Headers);
        var isTokenValid = await HeadersHelper.IsAdminTokenValid(token, _context);
        if (!isTokenValid) return BadRequest("You have no rights.");

        var itemsAmount = await _context.Items.CountAsync(u => u.UserId.ToString() == id);
        return itemsAmount;
    }
    
    [HttpGet]
    [Route("[action]/{id}")]
    public async Task<ActionResult<int>> GetUserStoragesCount(string id)
    {
        var token = HeadersHelper.ExtractTokenFromHeaders(Request.Headers);
        var isTokenValid = await HeadersHelper.IsAdminTokenValid(token, _context);
        if (!isTokenValid) return NotFound("You have no rights.");

        var storagesAmount = await _context.Storages.CountAsync(u => u.UserId.ToString() == id);
        return storagesAmount;
    }
    
    [HttpGet]
    [Route("[action]/{id}")]
    public async Task<ActionResult<int>> GetUserRootStoragesCount(string id)
    {
        var token = HeadersHelper.ExtractTokenFromHeaders(Request.Headers);
        var isTokenValid = await HeadersHelper.IsAdminTokenValid(token, _context);
        if (!isTokenValid) return NotFound("You have no rights.");

        var storagesAmount = await _context.Storages
            .CountAsync(u => u.UserId.ToString() == id
            && u.ParentStorageId == null);
        return storagesAmount;
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

    [HttpGet]
    [Route("[action]/{id}")]
    public async Task<ActionResult<StorageDTO>> GetStorageWithMaxItems(string id)
    {
        var token = HeadersHelper.ExtractTokenFromHeaders(Request.Headers);
        var isTokenValid = await HeadersHelper.IsAdminTokenValid(token, _context);
        if (!isTokenValid) return NotFound("You have no rights.");

        var storage = await _context.Storages
            .Include(i => i.Items)
            .Where(u => u.UserId.ToString() == id)
            .OrderByDescending(i => i.Items.Count)
            .FirstOrDefaultAsync();
        if (storage == null)
        {
            return null!;
        }
        return StorageDTO.ConvertEntity(storage);

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