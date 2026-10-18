namespace Domain.ValueObjects;

public readonly record struct Price
{
    public Price(decimal amount)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), "Price cannot be negative.");
        Amount = amount;
    }

    public decimal Amount { get; }
    public static Price Zero => new(0m);
    public static Price From(decimal amount) => new(amount);
    public override string ToString() => Amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
}
