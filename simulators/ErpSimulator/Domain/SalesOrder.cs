namespace ErpSimulator.Domain;

public sealed class SalesOrder
{
    public int Id { get; private set; }
    public string OrderNumber { get; private set; } = string.Empty;
    public int CustomerId { get; private set; }
    public DateTime OrderDateUtc { get; private set; }
    public string SourceSystem { get; private set; } = string.Empty;
    public string SourceSellerId { get; private set; } = string.Empty;
    public string ExternalOrderId { get; private set; } = string.Empty;
    public string RequestHash { get; private set; } = string.Empty;
    public string Currency { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }

    private SalesOrder()
    {
    }

    private readonly List<SalesOrderLine> _lines = new();
    public IReadOnlyCollection<SalesOrderLine> Lines => _lines.AsReadOnly();
    public decimal TotalAmount => _lines.Sum(x => x.LineTotal);

    public static SalesOrder Create(string orderNumber, int customerId, DateTime orderDateUtc, string sourceSystem, string sourceSellerId, string externalOrderId, string requestHash)
    {
        orderNumber = NormalizeOrderNumber(orderNumber);
        customerId = ValidateCustomerId(customerId);
        sourceSystem = NormalizeSourceSystem(sourceSystem);
        sourceSellerId = NormalizeSourceSeller(sourceSellerId);
        externalOrderId = NormalizeExternalOrder(externalOrderId);
        requestHash = NormalizeRequestHash(requestHash);
        orderDateUtc = NormalizeOrderDate(orderDateUtc);
        return new SalesOrder
        {
            OrderNumber = orderNumber,
            CustomerId = customerId,
            OrderDateUtc = orderDateUtc,
            SourceSystem = sourceSystem,
            SourceSellerId = sourceSellerId,
            ExternalOrderId = externalOrderId,
            RequestHash = requestHash,
            Currency = "TRY",
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void AddLine(int itemId, decimal quantity, decimal unitPrice)
    {
        var lineNumber = _lines.Count + 1;
        var salesOrderLine = SalesOrderLine.Create(itemId, lineNumber, quantity, unitPrice);
        _lines.Add(salesOrderLine);
    }


    private static int ValidateCustomerId(int customerId)
    {
        return customerId <= 0 ? throw new ArgumentOutOfRangeException(nameof(customerId), "Customer ID must be greater than zero.") : customerId;
    }
    private static string NormalizeOrderNumber(string orderNumber)
    {
        if (string.IsNullOrWhiteSpace(orderNumber)) throw new ArgumentException("Order number cannot be empty.", nameof(orderNumber));
        orderNumber = orderNumber.Trim();
        return orderNumber.Length > 50 ? throw new ArgumentException("Order number cannot exceed 50 characters.", nameof(orderNumber)) : orderNumber;
    }
    private static string NormalizeSourceSystem(string sourceSystem)
    {
        if (string.IsNullOrWhiteSpace(sourceSystem)) throw new ArgumentException("Source system cannot be empty.", nameof(sourceSystem));
        sourceSystem = sourceSystem.Trim();
        return sourceSystem.Length > 30 ? throw new ArgumentException("Source system cannot exceed 30 characters.", nameof(sourceSystem)) : sourceSystem;
    }
    private static string NormalizeSourceSeller(string sourceSellerId)
    {
        if (string.IsNullOrWhiteSpace(sourceSellerId)) throw new ArgumentException("Source seller ID cannot be empty.", nameof(sourceSellerId));
        sourceSellerId = sourceSellerId.Trim();
        return sourceSellerId.Length > 50 ? throw new ArgumentException("Source seller ID cannot exceed 50 characters.", nameof(sourceSellerId)) : sourceSellerId;
    }
    private static string NormalizeExternalOrder(string externalOrderId)
    {
        if (string.IsNullOrWhiteSpace(externalOrderId)) throw new ArgumentException("External order ID cannot be empty.", nameof(externalOrderId));
        externalOrderId = externalOrderId.Trim();
        return externalOrderId.Length > 100 ? throw new ArgumentException("External order ID cannot exceed 100 characters.", nameof(externalOrderId)) : externalOrderId;
    }
    private static string NormalizeRequestHash(string requestHash)
    {
        if (string.IsNullOrWhiteSpace(requestHash)) throw new ArgumentException("Request hash cannot be empty.", nameof(requestHash));
        requestHash = requestHash.Trim();
        if (requestHash.Length != 64) throw new ArgumentException("Request hash must be exactly 64 characters.", nameof(requestHash));
        return requestHash.Any(c => !Uri.IsHexDigit(c)) ? throw new ArgumentException("Request hash must be a valid hexadecimal string.", nameof(requestHash)) : requestHash;
    }
    private static DateTime NormalizeOrderDate(DateTime orderDateUtc)
    {
        if (orderDateUtc == default) throw new ArgumentException("Order date cannot be the default value.", nameof(orderDateUtc));
        return orderDateUtc.Kind != DateTimeKind.Utc ? throw new ArgumentException("Order date must be in UTC.", nameof(orderDateUtc)) : orderDateUtc;
    }
}