namespace Domain.Common;

public sealed record Error(string Code, string Description)
{
    public static Error None { get; } = new(string.Empty, string.Empty);

    public bool IsNone => this == None;
}
