namespace MarketplaceSimulator.Contracts;

public sealed class OrderLineResponse
{
    public string ItemCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}