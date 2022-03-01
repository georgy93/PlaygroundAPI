namespace Playground.Domain.Entities.Aggregates.BuyerAggregate
{
    public class FullName : ValueObject
    {
        private string _fullNameString;

        protected FullName() { }

        public FullName(string firstName, string surname, string lastName)
        {
            FirstName = Guard.Against.NullOrWhiteSpace(firstName, nameof(firstName), "firstName is not supplied");
            Surname = Guard.Against.NullOrWhiteSpace(surname, nameof(surname), "surname is not supplied");
            LastName = Guard.Against.NullOrWhiteSpace(lastName, nameof(lastName), "lastName is not supplied");
        }

        public string FirstName { get; private set; }

        public string Surname { get; private set; }

        public string LastName { get; private set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_fullNameString))
                _fullNameString = string.Join(' ', FirstName, Surname, LastName);

            return _fullNameString;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName;
            yield return Surname;
            yield return LastName;
        }

        public static implicit operator string(FullName self) => self.ToString();
    }
}