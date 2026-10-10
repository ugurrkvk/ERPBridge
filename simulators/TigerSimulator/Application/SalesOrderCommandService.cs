using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TigerSimulator.Domain;
using TigerSimulator.Persistence;
using TigerSimulator.Soap.Contracts;

namespace TigerSimulator.Application;

public sealed class SalesOrderCommandService(TigerDbContext context)
{
    public async Task<CreateSalesOrderResponse> CreateAsync(CreateSalesOrderRequest request)
    {

        if (request == null) throw new ArgumentNullException(nameof(request));
        var normalizedRequest = SalesOrderRequestNormalizer.Normalize(request);
        var hash = SalesOrderRequestHasher.Compute(normalizedRequest);
        var existingOrder = await context.SalesOrders
            .AsNoTracking()
            .Include(x => x.Lines)
            .SingleOrDefaultAsync(x => x.SourceSystem == normalizedRequest.SourceSystem && x.SourceSellerId == normalizedRequest.SourceSellerId && x.ExternalOrderId == normalizedRequest.ExternalOrderId);
        if (existingOrder != null)
        {
            if (existingOrder.RequestHash != hash) throw new InvalidOperationException($"Sales order with SourceSystem '{normalizedRequest.SourceSystem}', SourceSellerId '{normalizedRequest.SourceSellerId}' and ExternalOrderId '{normalizedRequest.ExternalOrderId}' already exists with a different request hash.");
            CreateSalesOrderResponse existingResponse = new()
            {
                OrderId = existingOrder.Id,
                OrderNumber = existingOrder.OrderNumber,
                ExternalOrderId = existingOrder.ExternalOrderId,
                TotalAmount = existingOrder.TotalAmount,
                AlreadyExists = true
            };
            return existingResponse;
        }
        var customer = await context.Customers
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Code == normalizedRequest.CustomerCode);
        if (customer == null) throw new InvalidOperationException($"Customer with code '{normalizedRequest.CustomerCode}' does not exist.");
        if (!customer.IsActive) throw new InvalidOperationException($"Customer with code '{normalizedRequest.CustomerCode}' is not active.");

        var orderNumber = "ORD-" + Guid.NewGuid().ToString("N");
        var order = SalesOrder.Create(
            orderNumber,
            customer.Id,
            normalizedRequest.OrderDateUtc,
            normalizedRequest.SourceSystem,
            normalizedRequest.SourceSellerId,
            normalizedRequest.ExternalOrderId,
            hash
        );
        foreach (var line in normalizedRequest.Lines)
        {
            var item = await context.Items.SingleOrDefaultAsync(x => x.Code == line.ItemCode);
            if(item == null) throw new InvalidOperationException($"Item with code '{line.ItemCode}' does not exist.");
            item.Reserve(line.Quantity);
            order.AddLine(item.Id, line.Quantity, line.UnitPrice);
        }
        context.SalesOrders.Add(order);
        try
        {

            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            context.ChangeTracker.Clear();
            throw new InvalidOperationException($"A concurrency conflict occurred while creating the sales order with SourceSystem '{normalizedRequest.SourceSystem}', SourceSellerId '{normalizedRequest.SourceSellerId}' and ExternalOrderId '{normalizedRequest.ExternalOrderId}'.", ex);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            context.ChangeTracker.Clear();
            var savedOrder = await context.SalesOrders
                .AsNoTracking()
                .Include(x => x.Lines)
                .SingleOrDefaultAsync(x => x.SourceSystem == normalizedRequest.SourceSystem && x.SourceSellerId == normalizedRequest.SourceSellerId && x.ExternalOrderId == normalizedRequest.ExternalOrderId);
            if (savedOrder == null)
            {
                throw;
            }
            if (savedOrder.RequestHash != hash) throw new InvalidOperationException($"Sales order with SourceSystem '{normalizedRequest.SourceSystem}', SourceSellerId '{normalizedRequest.SourceSellerId}' and ExternalOrderId '{normalizedRequest.ExternalOrderId}' already exists with a different request hash.");
            CreateSalesOrderResponse existingResponse = new()
            {
                OrderId = savedOrder.Id,
                OrderNumber = savedOrder.OrderNumber,
                ExternalOrderId = savedOrder.ExternalOrderId,
                TotalAmount = savedOrder.TotalAmount,
                AlreadyExists = true
            };
            return existingResponse;
        }
        CreateSalesOrderResponse createdResponse = new()
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            ExternalOrderId = order.ExternalOrderId,
            TotalAmount = order.TotalAmount,
            AlreadyExists = false
        };
        return createdResponse;
    }
}