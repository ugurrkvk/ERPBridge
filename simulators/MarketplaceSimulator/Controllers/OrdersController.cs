using Microsoft.AspNetCore.Mvc;
using MarketplaceSimulator.Application;
using MarketplaceSimulator.Contracts;

namespace MarketplaceSimulator.Controllers;

[Route("api/v1/sellers/{sellerId}/orders")]
[ApiController]
public class OrdersController(OrderQueryService orderQueryService) : ControllerBase
{
    [HttpGet]
    public ActionResult<OrderResponse[]> GetOrders(string sellerId)
    {
        var orders = orderQueryService.GetOrders(sellerId);
        return Ok(orders);
    }
}