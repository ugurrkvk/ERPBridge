namespace ERPBridge.Domain.Orders;

public sealed class OrderTransfer
{
    public Guid Id { get; private set; }
    public string SourceSystem { get; private set; } = string.Empty;
    public string SourceSellerId { get; private set; } = string.Empty;
    public string ExternalOrderId { get; private set; } = string.Empty;
    public string RequestHash { get; private set; } = string.Empty;
    public OrderTransferStatus Status { get; private set; }
    public int AttemptCount { get; private set; }
    public int? ErpOrderId { get; private set; }
    public string? ErpOrderNumber { get; private set; }
    public string? LastError { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? LastAttemptAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }

    public OrderTransfer()
    {
        
    }
}