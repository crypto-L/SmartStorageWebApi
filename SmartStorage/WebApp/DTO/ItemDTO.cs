using System;
using App.Domain;

namespace WebApp.DTO;

public class ItemDTO
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string? SerialNumber { get; set; }
    public byte[]? Image { get; set; }
    public string? Category { get; set; }
    public int? WeightInGrams { get; set; }
    public int? Amount { get; set; }

    public ItemDTO(Guid id, string title, string? serialNumber, byte[]? image, string? category,
        int? weight, int? amount)
    {
        Id = id.ToString();
        Title = title;
        SerialNumber = serialNumber;
        Image = image;
        Category = category;
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
        var title = entity.Title;
        var serialNumber = entity.SerialNumber;
        var image = entity.Image;
        var category = entity.Category;
        var weight = entity.WeightInGrams;
        var amount = entity.Amount;

        return new ItemDTO(id, title, serialNumber, image, category, weight, amount);
    }
}