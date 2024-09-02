using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.ValueObjects;

public sealed class Content : ValueObject
{
    public const int MaxValue = 10000;
    public string Value { get; }

    private Content(string value)
    {
        Value = value;
    }

    public static Result<Content> Create(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return Result.Failure<Content>(DomainErrors.Content.Empty);
        }

        if (content.Length > MaxValue)
        {
            return Result.Failure<Content>(DomainErrors.Content.TooLong);
        }

        return new Content(content);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
