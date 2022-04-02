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
        if (!isTokenValid) return BadRequest("You have no rights.");
        
        var storage = await _context.Storages
            .Include(st => st.ParentStorage)
            .Include(i => i.Items)
            .FirstOrDefaultAsync(s => s.UserId.ToString() == token.UserId
                                      && s.Id.ToString() == id);
        return storage != null
            ? StorageDTO.ConvertEntity(storage)
            : NotFound("Storage with id {id} does not exist.");
    }
    
    [HttpGet]
    [Route("[action]/{rootStorageId}")]
    public async Task<ActionResult<List<StorageDTO>>> GetAllStorages(string rootStorageId)
    {
        var token = HeadersHelper.ExtractTokenFromHeaders(Request.Headers);
        var isTokenValid = await HeadersHelper.IsTokenValid(token, _context);
        if (!isTokenValid) return BadRequest("You have no rights.");
        
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
    
    private static List<StorageDTO> ConvertStorageEntitiesToDTO(IEnumerable<Storage> storages)
    {
        return storages.Select(StorageDTO.ConvertEntity).ToList();
    }
}