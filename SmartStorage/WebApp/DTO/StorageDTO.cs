using System.Text.Json.Serialization;
using App.Domain;

namespace WebApp.DTO;

public class StorageDTO
{
    public string? Id { get; set; } 
    public string StorageName { get; set; }
    public string UserId { get; set; }
    public List<ItemDTO>? StorageItems { get; set; }
    public string? ParentId { get; set; }
    public string? ParentName { get; set; }
    public Dictionary<string, string>? SubStoragesIdNameDictionary { get; set; }

    [JsonConstructor]
    public StorageDTO(){}
    
    public StorageDTO(Guid id, string storageName, Guid userId, Guid? parentStorageId, 
        string? parentName, List<ItemDTO>? storageItems, Dictionary<string, string>? subStoragesIdNameDictionary)
    {
        Id = id.ToString();
        StorageName = storageName;
        UserId = userId.ToString();
        StorageItems = storageItems;
        
        ParentId = parentStorageId != null ? parentStorageId.ToString() : null;
        ParentName = parentName;
        SubStoragesIdNameDictionary = subStoragesIdNameDictionary;
    }

    public static StorageDTO ConvertEntity(Storage entity)
    {
        var id = entity.Id;
        var storageName = entity.StorageName;
        var userId = entity.UserId;
        var parentId = entity.ParentStorageId;
        var parentName = entity.ParentStorage?.StorageName;

        var items = entity.Items;
        List<ItemDTO>? storageItemsDtos = null;
        if (items != null)
        {
            storageItemsDtos = new List<ItemDTO>();
            foreach (var item in items)
            {
                var itemId = item.Id;
                var itemTitle = item.Title;
                var newItem = new ItemDTO(itemId, itemTitle);
                storageItemsDtos.Add(newItem);
            }
        }
        
        var subStorages = entity.SubStorages;
        Dictionary<string, string>? subStoragesIdNameDictionary = null;
        if (subStorages != null)
        {
            subStoragesIdNameDictionary = new Dictionary<string, string>();
            foreach (var subStorage in subStorages)
            {
                var subStorageId = subStorage.Id.ToString();
                var subStorageName = subStorage.StorageName;
                subStoragesIdNameDictionary.Add(subStorageId, subStorageName);
            }
        }
        return new StorageDTO(id, storageName, userId, parentId, parentName, storageItemsDtos, subStoragesIdNameDictionary);
    }
}