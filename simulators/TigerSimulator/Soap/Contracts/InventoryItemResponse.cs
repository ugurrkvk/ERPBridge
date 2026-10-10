namespace TigerSimulator.Soap.Contracts;

public sealed class InventoryItemResponse
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal AvailableQuantity { get; set; }
    public bool IsActive { get; set; }
}
