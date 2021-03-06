using System.Text.Json.Serialization;
using App.Domain;

namespace WebApp.DTO;

public class UserDTO
{
    public string? Id { get; set; }
    public string Nickname { get; set; }
    public string? PasswordHash { get; set; }
    public Dictionary<string,string>? RootStoragesIdNameDictionary { get; set; }
    
    //needed for binding data from post request
    [JsonConstructor]
    public UserDTO(){}
    
    public UserDTO(Guid id, string nickname, string passwordHash, Dictionary<string,string>? rootStoragesIdNameDictionary)
    {
        Id = id.ToString();
        Nickname = nickname;
        PasswordHash = passwordHash;
        RootStoragesIdNameDictionary = rootStoragesIdNameDictionary;
    }

    public UserDTO(Guid id, string nickname)
    {
        Id = id.ToString();
        Nickname = nickname;
    }

    public static UserDTO ConvertEntity(User entity)
    {
        var id = entity.Id;
        var nickname = entity.Nickname;
        var passwordHash = entity.PasswordHash;
        
        var allStorages = entity.UserStorages;
        Dictionary<string, string>? rootStoragesIdNameDictionary = null;
        if (allStorages != null && allStorages.Count > 0)
        {
            rootStoragesIdNameDictionary = new Dictionary<string, string>();
            foreach (var storage in allStorages)
            {
                //check if storage is root level storage
                if (storage.ParentStorageId == null || storage.ParentStorageId == Guid.Empty)
                {
                    var rootStorageId = storage.Id.ToString();
                    var rootStorageName = storage.StorageName;
                    rootStoragesIdNameDictionary.Add(rootStorageId,rootStorageName);
                }
            }
        }
        return new UserDTO(id, nickname, passwordHash, rootStoragesIdNameDictionary);
    }

    public static UserDTO ConvertEntityWithoutPassword(User entity)
    {
        var id = entity.Id;
        var nickname = entity.Nickname;

        return new UserDTO(id, nickname);
    }
}