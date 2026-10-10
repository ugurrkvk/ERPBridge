namespace ErpSimulator.Domain;

public sealed class Item
{
    public int Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Barcode { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public decimal OnHandQuantity { get; private set; }
    public decimal ReservedQuantity { get; private set; }
    public decimal AvailableQuantity => OnHandQuantity - ReservedQuantity;
    public bool IsActive { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();   

    private Item()
    {
        
    }

    public static Item Create(string code, string name, string barcode, decimal unitPrice, decimal onHandQuantity)
    {
        code = NormalizeCode(code);
        name = NormalizeName(name);
        barcode = NormalizeBarcode(barcode);
        unitPrice = NormalizeUnitPrice(unitPrice);
        onHandQuantity = NormalizeQuantity(onHandQuantity);
        return new Item
        {
            Code = code,
            Name = name,
            Barcode = barcode,
            UnitPrice = unitPrice,
            OnHandQuantity = onHandQuantity,
            ReservedQuantity = 0,
            IsActive = true
        };
    }

    public void Reserve(decimal quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity to reserve must be greater than zero.");
        EnsureActive();
        if (quantity > AvailableQuantity) throw new InvalidOperationException("Cannot reserve more than available quantity.");
        ReservedQuantity += quantity;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;


    private static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Code cannot be empty.", nameof(code));
        code = code.Trim();
        return code.Length > 50 ? throw new ArgumentException("Code cannot exceed 50 characters.", nameof(code)) : code;
    }
    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.", nameof(name));
        name = name.Trim();
        return name.Length > 200 ? throw new ArgumentException("Name cannot exceed 200 characters.", nameof(name)) : name;
    }
    private static string NormalizeBarcode(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode)) throw new ArgumentException("Barcode cannot be empty.", nameof(barcode));
        barcode = barcode.Trim();
        return barcode.Length > 50 ? throw new ArgumentException("Barcode cannot exceed 50 characters.", nameof(barcode)) : barcode;
    }
    private static decimal NormalizeUnitPrice(decimal unitPrice)
    {
        return unitPrice < 0 ? throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price cannot be negative.") : unitPrice;
    }
    private static decimal NormalizeQuantity(decimal quantity)
    {
        return quantity < 0 ? throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity cannot be negative.") : quantity;
    }
    private void EnsureActive()
    {
        if (!IsActive) throw new InvalidOperationException("Item must be active.");
    }
}