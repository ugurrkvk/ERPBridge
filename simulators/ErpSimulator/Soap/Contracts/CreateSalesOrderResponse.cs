namespace ErpSimulator.Soap.Contracts;

public sealed class CreateSalesOrderResponse
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string ExternalOrderId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public bool AlreadyExists { get; set; }
}