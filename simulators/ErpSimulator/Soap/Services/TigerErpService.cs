using ErpSimulator.Application;
using ErpSimulator.Soap.Contracts;

namespace ErpSimulator.Soap.Services;

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