using Microsoft.EntityFrameworkCore;
using TigerSimulator.Domain;

namespace TigerSimulator.Persistence;

public static class TigerDbSeeder
{
    public static async Task SeedAsync(TigerDbContext context)
    {
        var itemExists = await context.Items.AnyAsync(x => x.Code == "MOUSE-001");
        if (!itemExists)
        {
            var item = Item.Create("MOUSE-001", "Logitech M185 Mouse", "DEMO-M185", 750m, 25m);
            context.Items.Add(item);
        }


        var customerExists = await context.Customers.AnyAsync(x => x.Code == "CARI-001");
        if (!customerExists)
        {
            var customer = Customer.Create("CARI-001", "Uğur KAVAK", "Sultanbeyli / İstanbul — test adresi", null, null);
            context.Customers.Add(customer);
        }

        await context.SaveChangesAsync();
    }
}