using System;
using System.Text.Json.Serialization;
using App.Domain;

namespace WebApp.DTO;

public class ItemDTO
{
    public string? Id { get; set; }
    
    public string StorageId { get; set; }
    public string Title { get; set; }
    public string? SerialNumber { get; set; }
    public string? Image { get; set; }
    public string? Category { get; set; }
    public int? WeightInGrams { get; set; }
    public int? Amount { get; set; }
    
    [JsonConstructor]
    public ItemDTO(){}
    
    public ItemDTO(Guid id, Guid storageId, string title, string? serialNumber, string? image, string? category,
        int? weight, int? amount)
    {
        Id = id.ToString();
        StorageId = storageId.ToString();
        Title = title;
        SerialNumber = serialNumber;
        Image = image;
        Category = category;
        weight ??= 0;
        amount ??= 0;
        WeightInGrams = weight;
        Amount = amount;
    }

    public ItemDTO(Guid id, string title)
    {
        Id = id.ToString();
        Title = title;
    }
    

    public static ItemDTO ConvertEntity(Item entity)
    {
        var id = entity.Id;
        var storageId = entity.StorageId;
        var title = entity.Title;
        var serialNumber = entity.SerialNumber;

        var imageBytes = entity.Image;
        string? image = null;
        if (imageBytes != null)
        {
            image = Convert.ToBase64String(imageBytes);
        }
        
        var category = entity.Category;
        var weight = entity.WeightInGrams;
        var amount = entity.Amount;

        return new ItemDTO(id, storageId, title, serialNumber, image, category, weight, amount);
    }
}