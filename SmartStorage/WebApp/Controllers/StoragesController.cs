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
            var parenExist = await _context.Storages.AnyAsync(s => s.Id.ToString() == storage.ParentId);
            if (parenExist)
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
}