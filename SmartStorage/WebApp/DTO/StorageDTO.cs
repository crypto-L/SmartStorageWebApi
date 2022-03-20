using App.Domain;

namespace WebApp.DTO;

public class StorageDTO
{
    public string Id { get; set; }
    public string StorageName { get; set; }
    public string UserId { get; set; }
    public List<ItemDTO>? StorageItems { get; set; } = new List<ItemDTO>();
    public string? ParentId { get; set; }
    public Dictionary<string, string>? SubStoragesIdNameDictionary { get; set; } = new Dictionary<string, string>();

    public StorageDTO(Guid id, string storageName, Guid userId, Guid? parentStorageId, 
        string? parentName, Dictionary<string, string>? subStoragesIdNameDictionary, List<ItemDTO>? storageItems)
    {
        
    }

    public static StorageDTO ConvertEntity(Storage entity)
    {
        var id = entity.Id.ToString();
        var storageName = entity.StorageName;
        var parentId = entity.ParentStorageId.ToString();
        var userId = entity.UserId.ToString();
        
        //TODO: create itemsDTOs with minimal info set
        var items = entity.Items;
        
        //TODO: create dict with sub-storages IDs and names
        
        return null!;
    }
}