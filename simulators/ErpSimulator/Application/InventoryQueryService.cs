using Microsoft.EntityFrameworkCore;
using ErpSimulator.Persistence;
using ErpSimulator.Soap.Contracts;

namespace ErpSimulator.Application;

public sealed class InventoryQueryService(TigerDbContext context)
{
    public Task<InventoryItemResponse[]> GetInventoryAsync()
    {
        return context.Items
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .Select(x => new InventoryItemResponse
            {
                Code = x.Code,
                Name = x.Name,
                Barcode = x.Barcode,
                UnitPrice = x.UnitPrice,
                AvailableQuantity = x.OnHandQuantity - x.ReservedQuantity,
                IsActive = x.IsActive
            })
            .ToArrayAsync();
    }
}