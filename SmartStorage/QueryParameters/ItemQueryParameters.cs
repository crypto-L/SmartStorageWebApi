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
    
    public bool ValidWeightRange()
    {
        if (MaxWeight == null && MinWeight == null)
        {
            return true;
        }

        if (MaxWeight == null || MinWeight == null)
        {
            return true;
        }
        return MaxWeight > MinWeight;
    }

    public bool ValidAmountRange()
    {
        if (MaxAmount == null && MinAmount == null)
        {
            return true;
        }

        if (MaxAmount == null || MinAmount == null)
        {
            return true;
        }
        return MaxAmount > MinAmount;
    }
}