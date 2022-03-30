using System.Globalization;

namespace QueryParameters;

public class ItemQueryParameters
{
    public string? Title { get; set; }
    public string? SerialNumber { get; set; }
    public string? Category { get; set; }
    public int? MinWeight { get; set; }
    public int? MaxWeight { get; set; }
    public int? MinAmount { get; set; }
    public int? MaxAmount { get; set; }
    
    public bool ValidWeightRange => MaxWeight > MinWeight;
    public bool ValidAmountRange => MaxAmount > MinAmount;
    
}