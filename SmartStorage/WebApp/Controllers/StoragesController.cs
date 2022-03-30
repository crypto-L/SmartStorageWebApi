using App.DAL;
using App.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.DTO;
using WebApp.Helpers;

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

    [HttpGet("{id}")]
    public async Task<ActionResult<StorageDTO>> Get(string id)
    {
        var token = HeadersHelper.ExtractTokenFromHeaders(Request.Headers);
        var isTokenValid = await HeadersHelper.IsTokenValid(token, _context);
        
        if (isTokenValid)
        {
            var storage = await _context.Storages
                .Include(st => st.ParentStorage)
                .Include(i => i.Items)
                .FirstOrDefaultAsync(s => s.UserId.ToString() == token.UserId
                                 && s.Id.ToString() == id);
            return storage != null
                ? StorageDTO.ConvertEntity(storage)
                : NotFound("Storage with id {id} does not exist.");
        }
        return BadRequest("You have no rights.");
    }
    
    [HttpGet]
    [Route("[action]/{rootStorageId}")]
    public async Task<ActionResult<List<StorageDTO>>> GetAllStorages(string rootStorageId)
    {
        var token = HeadersHelper.ExtractTokenFromHeaders(Request.Headers);
        var isTokenValid = await HeadersHelper.IsTokenValid(token, _context);

        if (isTokenValid)
        {
            List<Storage> storages;
            
            //for initial page loading
            if (rootStorageId == "0")
            {
                storages = await _context.Storages
                    .Where(st => st.UserId.ToString() == token.UserId 
                                 && st.ParentStorageId == null)
                    .Include(sb => sb.SubStorages)
                    .ToListAsync();
                return ConvertStorageEntitiesToDTO(storages);
            }
            //for concrete storage
            storages = await _context.Storages
                .Where(st => st.UserId.ToString() == token.UserId
                             && st.ParentStorageId.ToString() == rootStorageId)
                .Include(i => i.Items)
                .Include(sb => sb.SubStorages)
                .ToListAsync();
            return ConvertStorageEntitiesToDTO(storages);
        }

        return BadRequest("You have no rights.");
    }

    // can create only empty storage/substorage related to some user
    //e.g DTO has only UserId, Name and ParentStorageId
    [HttpPost]
    public async Task<ActionResult<StorageDTO>> Post(StorageDTO storage)
    {
        var token = HeadersHelper.ExtractTokenFromHeaders(Request.Headers);
        var isTokenValid = await HeadersHelper.IsTokenValid(token, _context);
        if (!isTokenValid) return BadRequest("You have no rights.");
        
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
    
    private static List<StorageDTO> ConvertStorageEntitiesToDTO(IEnumerable<Storage> storages)
    {
        return storages.Select(StorageDTO.ConvertEntity).ToList();
    }
}