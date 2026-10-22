namespace Infrastructure.Persistence;

internal sealed class EgyptianDateTimeConverter() : ValueConverter<DateTimeOffset, DateTime>(
    value => value.UtcDateTime,
    value => EgyptianDateTime.ToCairo(new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc))));

internal sealed class NullableEgyptianDateTimeConverter() : ValueConverter<DateTimeOffset?, DateTime?>(
    value => value.HasValue ? value.Value.UtcDateTime : null,
    value => value.HasValue ? EgyptianDateTime.ToCairo(new DateTimeOffset(DateTime.SpecifyKind(value.Value, DateTimeKind.Utc))) : null);

internal sealed class PriceConverter() : ValueConverter<Price, decimal>(value => value.Amount, value => new Price(value));
