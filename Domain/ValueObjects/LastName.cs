using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class LastName : ValueObject
{
    public const int MaxLength = 50;

    public string Value { get; }

    private LastName(string value)
    {
        Value = value;
    }

    public static Result<LastName> Create(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result.Failure<LastName>(DomainErrors.LastName.Empty);
        }

        if (firstName.Length > MaxLength)
        {
            return Result.Failure<LastName>(DomainErrors.LastName.TooLong);
        }

        return new LastName(firstName);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
