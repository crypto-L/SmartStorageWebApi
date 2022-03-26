using App.DAL;
using App.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.DTO;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoragesController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public StoragesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StorageDTO>>> Get()
    {
        var header = Request.Headers;
        var token = ExtractTokenFromHeaders(header);
        var isTokenValid = await IsTokenValid(token);
        Console.WriteLine($"Token: {token.Token}");
        Console.WriteLine($"User: {token.UserId}");
        Console.WriteLine(isTokenValid.ToString());
        //TODO: get all root or substorages (using parentStorageId)
        
        var storages = await _context.Storages
            .Include(s => s.Items)
            .ToListAsync();
        var storageDtos = new List<StorageDTO>();
        foreach (var storage in storages)
        {
            var storageDto = StorageDTO.ConvertEntity(storage);
            storageDtos.Add(storageDto);
        }
        return storageDtos;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<StorageDTO>> Get(string id)
    {
        var storage = await _context.Storages
            .Where(s => s.Id.ToString() == id)
            .Include(p => p.ParentStorage)
            .Include(i => i.Items)
            .Include(st => st.SubStorages)
            .FirstOrDefaultAsync();
        if (storage == null)
        {
            return NotFound($"Storage with id {id} does not exist.");
        }
        return StorageDTO.ConvertEntity(storage);
    }

    // can create only empty storage/substorage related to some user
    //e.g DTO has only UserId, Name and ParentStorageId
    [HttpPost]
    public async Task<ActionResult<StorageDTO>> Post(StorageDTO storage)
    {
        var storageEntity = new Storage()
        {
            UserId = Guid.Parse(storage.UserId),
            StorageName = storage.StorageName
        };
        if (storage.ParentId != null)
        {
            var parentExist = await _context.Storages.AnyAsync(s => s.Id.ToString() == storage.ParentId);
            if (parentExist)
            {
                storageEntity.ParentStorageId = Guid.Parse(storage.ParentId);
            }
            else
            {
                return BadRequest($"Parent storage with id {storage.ParentId} does not exist.");
            }
        }
        await _context.Storages.AddAsync(storageEntity);
        await _context.SaveChangesAsync();
        return StorageDTO.ConvertEntity(storageEntity);
    }

    //only changes storage name
    [HttpPut("{id}")]
    public async Task<ActionResult<StorageDTO>> Put(string id, StorageDTO storage)
    {
        var storageExist = await _context.Storages.AnyAsync(s => s.Id.ToString() == id);
        if (storageExist)
        {
            var storageEntity = await _context.Storages.FirstAsync(s => s.Id.ToString() == id);
            storageEntity.StorageName = storage.StorageName;
            await _context.SaveChangesAsync();
            var storageDto = StorageDTO.ConvertEntity(storageEntity);
            return Ok(storageDto);
        }

        return BadRequest("Something goes wrong...");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<StorageDTO>> Delete(string id)
    {
        var storageExists = await _context.Storages.AnyAsync(s => s.Id.ToString() == id);
        if (storageExists)
        {
            var storage = await _context.Storages.FirstAsync(s => s.Id.ToString() == id);
            var storageDto = StorageDTO.ConvertEntity(storage);
            _context.Storages.Remove(storage);
            await _context.SaveChangesAsync();
            return Ok(storageDto);
        }
        
        return BadRequest($"Storage with id {id} does not exist.");
    }

    private static TokenDTO ExtractTokenFromHeaders(IHeaderDictionary headers)
    {
        headers.TryGetValue("userId", out var userId);
        headers.TryGetValue("token", out var tokenString);
        var tokenDto = new TokenDTO(userId, tokenString);
        return tokenDto;
    }

    private async Task<bool> IsTokenValid(TokenDTO token)
    {
        if (token.Token ==  null|| token.UserId == null)
        {
            return false;
        }

        return await _context.Users
            .Include(t => t.Token)
            .AnyAsync
            (u => u.Id.ToString() == token.UserId 
                  && u.Token != null
                  && u.Token.TokenString == token.Token);
    }
}