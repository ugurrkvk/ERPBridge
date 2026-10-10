namespace ErpSimulator.Soap.Contracts;

public sealed class CreateSalesOrderRequest
{
    public string CustomerCode { get; set; } = string.Empty;
    public DateTime OrderDateUtc { get; set; }
    public string SourceSystem { get; set; } = string.Empty;
    public string SourceSellerId { get; set; } = string.Empty;
    public string ExternalOrderId { get; set; } = string.Empty;
    public CreateSalesOrderLineRequest[] Lines { get; set; } = Array.Empty<CreateSalesOrderLineRequest>();
}