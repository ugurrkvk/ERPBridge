using TigerSimulator.Soap.Contracts;

namespace TigerSimulator.Application;

public static class SalesOrderRequestNormalizer
{
    public static CreateSalesOrderRequest Normalize(CreateSalesOrderRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        var customerCode = NormalizeCustomerCode(request.CustomerCode);
        var orderDateUtc = ValidateOrderDateUtc(request.OrderDateUtc);
        var sourceSystem = NormalizeSourceSystem(request.SourceSystem);
        var sourceSellerId = NormalizeSourceSellerId(request.SourceSellerId);
        var externalOrderId = NormalizeExternalOrderId(request.ExternalOrderId);
        var lines = NormalizeLines(request.Lines);

        return new CreateSalesOrderRequest
        {
            CustomerCode = customerCode,
            OrderDateUtc = orderDateUtc,
            SourceSystem = sourceSystem,
            SourceSellerId = sourceSellerId,
            ExternalOrderId = externalOrderId,
            Lines = lines
        };
    }


    private static string NormalizeCustomerCode(string customerCode)
    {
        if (string.IsNullOrWhiteSpace(customerCode)) throw new ArgumentException("Customer code cannot be null or whitespace.", nameof(customerCode));
        customerCode = customerCode.Trim();
        return customerCode.Length > 50 ? throw new ArgumentException("Customer code cannot exceed 50 characters.", nameof(customerCode)) : customerCode;
    }
    private static string NormalizeSourceSystem(string sourceSystem)
    {
        if (string.IsNullOrWhiteSpace(sourceSystem)) throw new ArgumentException("Source system cannot be null or whitespace.", nameof(sourceSystem));
        sourceSystem = sourceSystem.Trim();
        return sourceSystem.Length > 30 ? throw new ArgumentException("Source system cannot exceed 30 characters.", nameof(sourceSystem)) : sourceSystem;
    }
    private static string NormalizeSourceSellerId(string sourceSellerId)
    {
        if (string.IsNullOrWhiteSpace(sourceSellerId)) throw new ArgumentException("Source seller ID cannot be null or whitespace.", nameof(sourceSellerId));
        sourceSellerId = sourceSellerId.Trim();
        return sourceSellerId.Length > 50 ? throw new ArgumentException("Source seller ID cannot exceed 50 characters.", nameof(sourceSellerId)) : sourceSellerId;
    }
    private static string NormalizeExternalOrderId(string externalOrderId)
    {
        if (string.IsNullOrWhiteSpace(externalOrderId)) throw new ArgumentException("External order ID cannot be null or whitespace.", nameof(externalOrderId));
        externalOrderId = externalOrderId.Trim();
        return externalOrderId.Length > 100 ? throw new ArgumentException("External order ID cannot exceed 100 characters.", nameof(externalOrderId)) : externalOrderId;
    }
    private static DateTime ValidateOrderDateUtc(DateTime orderDateUtc)
    {
        if (orderDateUtc == default) throw new ArgumentException("Order date cannot be the default value.", nameof(orderDateUtc));
        return orderDateUtc.Kind != DateTimeKind.Utc ? throw new ArgumentException("Order date must be in UTC.", nameof(orderDateUtc)) : orderDateUtc;
    }
    private static CreateSalesOrderLineRequest[] NormalizeLines(CreateSalesOrderLineRequest[] lines)
    {
        if (lines == null) throw new ArgumentNullException(nameof(lines));
        if (lines.Length == 0) throw new ArgumentException("Lines cannot be empty.", nameof(lines));
        var salesOrderLines = new List<CreateSalesOrderLineRequest>();
        foreach (var line in lines)
        {
            if (line == null) throw new ArgumentException("Line cannot be null.", nameof(lines));
            if (string.IsNullOrWhiteSpace(line.ItemCode)) throw new ArgumentException("Item code cannot be null or whitespace.", nameof(line.ItemCode));
            var itemCode = line.ItemCode.Trim();
            if (itemCode.Length > 50) throw new ArgumentException("Item code cannot exceed 50 characters.", nameof(line.ItemCode));
            if (line.Quantity <= 0) throw new ArgumentOutOfRangeException(nameof(line.Quantity), "Quantity must be greater than zero.");
            if (line.UnitPrice < 0) throw new ArgumentOutOfRangeException(nameof(line.UnitPrice), "Unit price cannot be negative.");
            salesOrderLines.Add(new CreateSalesOrderLineRequest
            {
                ItemCode = itemCode,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice
            });
        }
        return [.. salesOrderLines];
    }
}