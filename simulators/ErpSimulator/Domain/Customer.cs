namespace ErpSimulator.Domain;

public sealed class Customer
{
    public int Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private Customer()
    {

    }

    public static Customer Create(string code, string name, string address, string? email, string? phoneNumber)
    {
        code = NormalizeCode(code);
        name = NormalizeName(name);
        address = NormalizeAddress(address);
        email = NormalizeEmail(email);
        phoneNumber = NormalizePhoneNumber(phoneNumber);
        return new Customer
        {
            Code = code,
            Name = name,
            Address = address,
            Email = email,
            PhoneNumber = phoneNumber,
            IsActive = true
        };
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
    private static string NormalizeAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address)) throw new ArgumentException("Address cannot be empty.", nameof(address));
        address = address.Trim();
        return address.Length > 200 ? throw new ArgumentException("Address cannot exceed 200 characters.", nameof(address)) : address;
    }
    private static string NormalizeEmail(string? email)
    {
        email = email?.Trim() ?? string.Empty;
        return email.Length > 100 ? throw new ArgumentException("Email cannot exceed 100 characters.", nameof(email)) : email;
    }
    private static string NormalizePhoneNumber(string? phoneNumber)
    {
        phoneNumber = phoneNumber?.Trim() ?? string.Empty;
        return phoneNumber.Length > 20 ? throw new ArgumentException("Phone number cannot exceed 20 characters.", nameof(phoneNumber)) : phoneNumber;
    }
}
