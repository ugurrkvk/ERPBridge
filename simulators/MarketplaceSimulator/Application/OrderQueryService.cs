using MarketplaceSimulator.Contracts;

namespace MarketplaceSimulator.Application;

public sealed class OrderQueryService
{
    public OrderResponse[] GetOrders(string sellerId)
    {
        return new[]
        {
            new OrderResponse()
            {
                ExternalOrderId = "TY-DEMO-2001",
                SellerId = "SELLER-001",
                CustomerCode = "CARI-001",
                OrderDateUtc = new DateTime(2026, 10, 7, 12, 0, 0, DateTimeKind.Utc),
                Lines =
                [
                    new OrderLineResponse
                    {
                        ItemCode = "MOUSE-001",
                        Quantity = 2m,
                        UnitPrice = 750m
                    }
                ]
            }
        }.Where(order => order.SellerId == sellerId).ToArray();
    }
}