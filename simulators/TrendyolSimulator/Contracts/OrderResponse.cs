namespace TrendyolSimulator.Contracts;

public sealed class OrderResponse
{
    public string ExternalOrderId { get; set; } = string.Empty;
    public string SellerId { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public DateTime OrderDateUtc { get; set; }
    public OrderLineResponse[] Lines { get; set; } = Array.Empty<OrderLineResponse>();
}