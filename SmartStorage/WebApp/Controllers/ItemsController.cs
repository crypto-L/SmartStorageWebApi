using App.DAL;
using App.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.DTO;

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
        var item = await _context.Items.Where(i => i.Id.ToString() == id).FirstOrDefaultAsync();
        if (item == null)
        {
            return NotFound($"Storage with id {id} does not exist.");
        }
        return ItemDTO.ConvertEntity(item);
    }

    [HttpGet]
    [Route("[action]/{storageId}")]
    public async Task<ActionResult<IEnumerable<ItemDTO>>> GetStorageItems(string storageId)
    {
        var storageExists = await _context.Storages.AnyAsync(s => s.Id.ToString() == storageId);
        List<ItemDTO> items;
        if (storageExists)
        {
            items = new List<ItemDTO>();
            var storage = await _context.Storages
                .Where(s => s.Id.ToString() == storageId)
                .Include(i => i.Items).FirstOrDefaultAsync();
            
            foreach (var item in storage.Items)
            {
                var itemDto = ItemDTO.ConvertEntity(item);
                items.Add(itemDto);
            }
            return items;
        }
        return NotFound($"Storage with id {storageId} does not exist.");
    }

    [HttpPost]
    public async Task<ActionResult<ItemDTO>> Post(ItemDTO item)
    {
        var storageExists = await _context.Storages.AnyAsync(s => s.Id.ToString() == item.StorageId);
        if (storageExists)
        {
            var itemEntity = new Item()
            {
                StorageId = Guid.Parse(item.StorageId),
                Title = item.Title,
                SerialNumber = item.SerialNumber,
                Image = item.Image,
                Category = item.Category,
                WeightInGrams = item.WeightInGrams,
                Amount = item.Amount
            };
            await _context.Items.AddAsync(itemEntity);
            await _context.SaveChangesAsync();
            return ItemDTO.ConvertEntity(itemEntity);
        }
        return BadRequest($"Storage with id {item.StorageId} does not exist. Can't save item.");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ItemDTO>> Delete(string id)
    {
        var itemExists = await _context.Items.AnyAsync(i => i.Id.ToString() == id);
        if (itemExists)
        {
            var item = await _context.Items.FirstAsync(i => i.Id.ToString() == id);
            var itemDto = ItemDTO.ConvertEntity(item);
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return Ok(itemDto);
        }
        return BadRequest($"Can't find item with id {id}.");
    }
}