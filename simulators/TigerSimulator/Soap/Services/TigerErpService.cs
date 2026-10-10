using TigerSimulator.Application;
using TigerSimulator.Soap.Contracts;

namespace TigerSimulator.Soap.Services;

public sealed class TigerErpService(InventoryQueryService inventoryQueryService, SalesOrderCommandService salesOrderCommandService) : ITigerErpService
{
    public Task<InventoryItemResponse[]> GetInventoryAsync()
    {
        return inventoryQueryService.GetInventoryAsync();
    }

    public Task<CreateSalesOrderResponse> CreateSalesOrderAsync(CreateSalesOrderRequest request)
    {
        return salesOrderCommandService.CreateAsync(request);
    }
}