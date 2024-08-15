namespace Playground.Domain.Entities.Aggregates.BuyerAggregate;

public class UserId : ValueObject
{
    protected UserId() { }

    public UserId(Guid value)
    {
        Value = Guard.Against.Default(value);
    }

    public Guid Value { get; private set; }

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator Guid(UserId self) => self.Value;

    public static implicit operator string(UserId self) => self.ToString();
}