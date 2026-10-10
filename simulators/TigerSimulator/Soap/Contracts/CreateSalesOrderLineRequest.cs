namespace TigerSimulator.Soap.Contracts;

public sealed class CreateSalesOrderLineRequest
{
    public string ItemCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}