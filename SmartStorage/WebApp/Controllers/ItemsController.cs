using App.DAL;
using App.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QueryParameters;
using WebApp.DTO;
using WebApp.Helpers;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : Controller
{
    private readonly AppDbContext _context;
    
    public ItemsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDTO>> Get(string id)
    {
        var token = HeadersHelper.ExtractTokenFromHeaders(Request.Headers);
        var isTokenValid = await HeadersHelper.IsTokenValid(token, _context);
        if (!isTokenValid) return BadRequest("You have no rights.");
        
        var item = await _context.Items
            .Where(i => i.UserId.ToString() == token.UserId 
                        && i.Id.ToString() == id)
            .FirstOrDefaultAsync();
        if (item == null)
        {
            return NotFound($"Storage with id {id} does not exist.");
        }
        return ItemDTO.ConvertEntity(item);
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<IEnumerable<ItemDTO>>> GetAll([FromQuery] ItemQueryParameters filter)
    {
        var token = HeadersHelper.ExtractTokenFromHeaders(Request.Headers);
        var isTokenValid = await HeadersHelper.IsTokenValid(token, _context);
        if (!isTokenValid) return BadRequest("You have no rights.");
        
        if (!filter.ValidAmountRange() || !filter.ValidWeightRange())
        {
            return NotFound("The minimum value must be less than the maximum.");
        }
        
        var titleFilter = filter.Title ?? "";
        var serialNumberFilter = filter.SerialNumber ?? "";
        var categoryFilter = filter.Category ?? "";
        var weightMinFilter = filter.MinWeight ?? -1;
        var weightMaxFilter = filter.MaxWeight ?? int.MaxValue;
        var amountMinFilter = filter.MinAmount ?? -1;
        var amountMaxFilter = filter.MaxAmount ?? int.MaxValue;

        var items = await _context.Items
            .Where(i => i.UserId.ToString() == token.UserId
            && i.Title.Contains(titleFilter)
            && i.SerialNumber.Contains(serialNumberFilter)
            && i.Category.Contains(categoryFilter)
            && i.WeightInGrams >= weightMinFilter && i.WeightInGrams <= weightMaxFilter
            && i.Amount >= amountMinFilter && i.Amount <= amountMaxFilter)
            .ToListAsync();
        
        return ConvertItemsEntitiesToDTO(items);;
    }
    
    [HttpPost]
    public async Task<ActionResult<ItemDTO>> Post(ItemDTO item)
    {
        var token = HeadersHelper.ExtractTokenFromHeaders(Request.Headers);
        var isTokenValid = await HeadersHelper.IsTokenValid(token, _context);
        if (!isTokenValid) return BadRequest("You have no rights.");
        
        var storageExists = await _context.Storages
            .AnyAsync(s => s.Id.ToString() == item.StorageId 
                           && s.UserId.ToString() == token.UserId);
        if (!storageExists) return BadRequest($"Storage with id {item.StorageId} does not exist. Can't save item.");
        
        byte[]? imageBytes = null;
        if (item.Image != null)
        {
            imageBytes = Convert.FromBase64String(item.Image);
        }
        
        var itemEntity = new Item()
        {
            StorageId = Guid.Parse(item.StorageId),
            UserId = Guid.Parse(token.UserId),
            Title = item.Title,
            SerialNumber = item.SerialNumber,
            Image = imageBytes,
            Category = item.Category,
            WeightInGrams = item.WeightInGrams ?? 0,
            Amount = item.Amount ?? 0
        };
        await _context.Items.AddAsync(itemEntity);
        await _context.SaveChangesAsync();
        return ItemDTO.ConvertEntity(itemEntity);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult<string>> Delete(ItemDTO itemDto)
    {
        var token = HeadersHelper.ExtractTokenFromHeaders(Request.Headers);
        var isTokenValid = await HeadersHelper.IsTokenValid(token, _context);
        if (!isTokenValid) return BadRequest("You have no rights.");
        
        var itemEntity = await _context.Items
            .FirstOrDefaultAsync(i => i.Id.ToString() == itemDto.Id 
                                      && i.StorageId.ToString() == itemDto.StorageId);
        if (itemEntity == null) return BadRequest($"Can't find item with id {itemDto.Id}.");
        
        _context.Items.Remove(itemEntity);
        await _context.SaveChangesAsync();
        
        return Ok($"Item {itemDto.Id} deleted.");
    }

    [HttpPut]
    public async Task<ActionResult<ItemDTO>> Put(ItemDTO itemDto)
    {
        var token = HeadersHelper.ExtractTokenFromHeaders(Request.Headers);
        var isTokenValid = await HeadersHelper.IsTokenValid(token, _context);
        if (!isTokenValid) return BadRequest("You have no rights.");

        var itemEntity = await _context.Items
            .FirstOrDefaultAsync(i => i.Id.ToString() == itemDto.Id
                                      && i.StorageId.ToString() == itemDto.StorageId);
        
        if (itemEntity == null) return BadRequest($"Can't find item with id {itemDto.Id}");

        byte[]? imageBytes = null;
        if (itemDto.Image != null)
        {
            imageBytes = Convert.FromBase64String(itemDto.Image);
        }
        itemEntity.Title = itemDto.Title;
        itemEntity.SerialNumber = itemDto.SerialNumber;
        itemEntity.Image = imageBytes;
        itemEntity.Category = itemDto.Category;
        itemEntity.WeightInGrams = itemDto.WeightInGrams;
        itemEntity.Amount = itemDto.Amount;

        await _context.SaveChangesAsync();
        return ItemDTO.ConvertEntity(itemEntity);
        
    }
    
    private static List<ItemDTO> ConvertItemsEntitiesToDTO(IEnumerable<Item> items)
    {
        return items.Select(ItemDTO.ConvertEntity).ToList();
    }
}