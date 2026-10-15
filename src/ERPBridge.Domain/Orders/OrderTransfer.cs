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

    private OrderTransfer()
    {
        
    }

    public static OrderTransfer Create(string sourceSystem, string sourceSellerId, string externalOrderId, string requestHash)
    {
        sourceSystem = NormalizeSourceSystem(sourceSystem);
        sourceSellerId = NormalizeSourceSellerId(sourceSellerId);
        externalOrderId = NormalizeExternalOrderId(externalOrderId);
        requestHash = NormalizeRequestHash(requestHash);
        var orderTransfer = new OrderTransfer
        {
            Id = Guid.NewGuid(),
            SourceSystem = sourceSystem,
            SourceSellerId = sourceSellerId,
            ExternalOrderId = externalOrderId,
            RequestHash = requestHash,
            Status = OrderTransferStatus.Pending,
            AttemptCount = 0,
            CreatedAtUtc = DateTime.UtcNow
        };

        return orderTransfer;
    }

    public void StartAttempt()
    {
        if (Status is not (OrderTransferStatus.Pending or OrderTransferStatus.Failed)) 
            throw new InvalidOperationException("Only pending or failed transfers can start an attempt.");
        Status = OrderTransferStatus.Processing;
        AttemptCount++;
        LastAttemptAtUtc = DateTime.UtcNow;
        LastError = null;
    }

    public void MarkSucceeded(int erpOrderId, string erpOrderNumber)
    {
        if (Status is not OrderTransferStatus.Processing) throw new InvalidOperationException("Cannot mark a non-processing order transfer as succeeded.");
        if (erpOrderId <= 0) throw new ArgumentOutOfRangeException(nameof(erpOrderId), "ERP order ID cannot be zero or negative.");
        if (string.IsNullOrWhiteSpace(erpOrderNumber)) throw new ArgumentException("ERP order number cannot be null or empty.", nameof(erpOrderNumber));
        erpOrderNumber = erpOrderNumber.Trim();
        if (erpOrderNumber.Length > 50) throw new ArgumentException("ERP order number cannot be longer than 50 characters.", nameof(erpOrderNumber));
        ErpOrderId = erpOrderId;
        ErpOrderNumber = erpOrderNumber;
        Status = OrderTransferStatus.Succeeded;
        CompletedAtUtc = DateTime.UtcNow;
        LastError = null;
    }

    public void MarkFailed(string error)
    {
        if (Status is not OrderTransferStatus.Processing) throw new InvalidOperationException("Cannot mark a non-processing order transfer as failed.");
        if (string.IsNullOrWhiteSpace(error)) throw new ArgumentException("Error cannot be null or empty.", nameof(error));
        error = error.Trim();
        if (error.Length > 2000) error = error[..2000];
        Status = OrderTransferStatus.Failed;
        LastError = error;
    }

    private static string NormalizeSourceSystem(string sourceSystem)
    {
        if (string.IsNullOrWhiteSpace(sourceSystem)) throw new ArgumentException("Source system cannot be null or empty.", nameof(sourceSystem));
        sourceSystem = sourceSystem.Trim();
        return sourceSystem.Length > 30 ? throw new ArgumentException("Source system cannot be longer than 30 characters.", nameof(sourceSystem)) : sourceSystem;
    }
    private static string NormalizeSourceSellerId(string sourceSellerId)
    {
        if (string.IsNullOrWhiteSpace(sourceSellerId)) throw new ArgumentException("Source seller ID cannot be null or empty.", nameof(sourceSellerId));
        sourceSellerId = sourceSellerId.Trim();
        return sourceSellerId.Length > 50 ? throw new ArgumentException("Source seller ID cannot be longer than 50 characters.", nameof(sourceSellerId)) : sourceSellerId;
    }
    private static string NormalizeExternalOrderId(string externalOrderId)
    {
        if (string.IsNullOrWhiteSpace(externalOrderId)) throw new ArgumentException("External order ID cannot be null or empty.", nameof(externalOrderId));
        externalOrderId = externalOrderId.Trim();
        return externalOrderId.Length > 100 ? throw new ArgumentException("External order ID cannot be longer than 100 characters.", nameof(externalOrderId)) : externalOrderId;
    }
    private static string NormalizeRequestHash(string requestHash)
    {
        if (string.IsNullOrWhiteSpace(requestHash)) throw new ArgumentException("Request hash cannot be empty.", nameof(requestHash));
        requestHash = requestHash.Trim();
        if (requestHash.Length != 64) throw new ArgumentException("Request hash must be exactly 64 characters.", nameof(requestHash));
        return requestHash.Any(c => !Uri.IsHexDigit(c)) ? throw new ArgumentException("Request hash must be a valid hexadecimal string.", nameof(requestHash)) : requestHash;
    }
}