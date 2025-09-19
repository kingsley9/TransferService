namespace TransferService.Domain.Entities
{
    public class Address
    {
        public string Street { get; } = string.Empty;
        public string City { get; } = string.Empty;
        public string State { get; } = string.Empty;
        public string PostalCode { get; } = string.Empty;
        public string Country { get; } = string.Empty;

        public Address() { }

        public Address(string street, string city, string state, string postalCode, string country)
        {
            Street = street;
            City = city;
            State = state;
            PostalCode = postalCode;
            Country = country;
        }

        public override bool Equals(object? obj)
        {
            return obj is Address other
                && Street == other.Street
                && City == other.City
                && State == other.State
                && PostalCode == other.PostalCode
                && Country == other.Country;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Street, City, State, PostalCode, Country);
        }

        public override string ToString() => $"{Street}, {City}, {State}, {PostalCode}, {Country}";
    }
}
