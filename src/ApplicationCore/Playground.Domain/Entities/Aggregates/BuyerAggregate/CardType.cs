namespace Playground.Domain.Entities.Aggregates.BuyerAggregate
{
    public class CardType : SmartEnum<CardType>
    {
        public static readonly CardType Amex = new("Amex", 1);
        public static readonly CardType Visa = new("Visa", 2);
        public static readonly CardType MasterCard = new("MasterCard", 3);

        protected CardType(string name, int value) : base(name, value) { }
    }
}