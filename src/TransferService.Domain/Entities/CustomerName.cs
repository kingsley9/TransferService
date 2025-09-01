using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferService.Domain.Entities
{
    public class CustomerName
    {
        public string FirstName { get; private set; } = String.Empty;
        public string MiddleName { get; private set; } = String.Empty;
        public string LastName { get; private set; } = String.Empty;

        public CustomerName() { }

        public CustomerName(string firstName, string middleName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty.", nameof(firstName));

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty.", nameof(lastName));

            if (firstName.Length > 50)
                throw new ArgumentException("First name is too long.", nameof(firstName));

            if (lastName.Length > 50)
                throw new ArgumentException("Last name is too long.", nameof(lastName));

            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
        }

        public override bool Equals(object? obj)
        {
            return obj is CustomerName other
                && FirstName == other.FirstName
                && MiddleName == other.MiddleName
                && LastName == other.LastName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FirstName, MiddleName, LastName);
        }

        public override string ToString() => $"{FirstName} {MiddleName} {LastName}";
    }
}
