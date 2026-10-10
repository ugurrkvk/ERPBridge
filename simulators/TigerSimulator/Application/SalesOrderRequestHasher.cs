using System.Globalization;
using TigerSimulator.Soap.Contracts;

namespace TigerSimulator.Application;

public static class SalesOrderRequestHasher
{
    public static string Compute(CreateSalesOrderRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        var normalizedRequest = SalesOrderRequestNormalizer.Normalize(request);
        var canonicalRequest = new { 
            CustomerCode = normalizedRequest.CustomerCode,
            OrderDateUtc = normalizedRequest.OrderDateUtc.ToString("O", CultureInfo.InvariantCulture),
            SourceSystem = normalizedRequest.SourceSystem,
            SourceSellerId = normalizedRequest.SourceSellerId,
            ExternalOrderId = normalizedRequest.ExternalOrderId,
            Lines = normalizedRequest.Lines.Select(line => new
            {
                ItemCode = line.ItemCode,
                Quantity = line.Quantity.ToString("G29",CultureInfo.InvariantCulture),
                UnitPrice = line.UnitPrice.ToString("G29", CultureInfo.InvariantCulture)
            }).ToArray()
        };
        var json = System.Text.Json.JsonSerializer.Serialize(canonicalRequest);
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(hashBytes);
    }
}