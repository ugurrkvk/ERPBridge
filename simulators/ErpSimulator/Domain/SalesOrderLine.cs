namespace ErpSimulator.Domain;

public sealed class SalesOrderLine
{
    public int Id { get; private set; }
    public int SalesOrderId { get; private set; }
    public int ItemId { get; private set; }
    public int LineNumber { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal LineTotal => Quantity * UnitPrice;

    private SalesOrderLine()
    {

    }
    public static SalesOrderLine Create(int itemId, int lineNumber, decimal quantity, decimal unitPrice)
    {
        itemId = NormalizeItem(itemId);
        lineNumber = NormalizeLineNumber(lineNumber);
        quantity = NormalizeQuantity(quantity);
        unitPrice = NormalizeUnitPrice(unitPrice);
        return new SalesOrderLine
        {
            ItemId = itemId,
            LineNumber = lineNumber,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }
    private static int NormalizeItem(int itemId)
    {
        return itemId <= 0 ? throw new ArgumentOutOfRangeException(nameof(itemId), "Item ID must be greater than zero.") : itemId;
    }
    private static int NormalizeLineNumber(int lineNumber)
    {
        return lineNumber <= 0 ? throw new ArgumentOutOfRangeException(nameof(lineNumber), "Line number must be greater than zero.") : lineNumber;
    }
    private static decimal NormalizeQuantity(decimal quantity)
    {
        return quantity <= 0 ? throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.") : quantity;
    }
    private static decimal NormalizeUnitPrice(decimal unitPrice)
    {
        return unitPrice < 0 ? throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price cannot be negative.") : unitPrice;
    }
}