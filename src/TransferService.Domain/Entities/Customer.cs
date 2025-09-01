using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferService.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; private set; }
        public CustomerName Name { get; private set; } = new();
        public DateTime DateOfBirth { get; private set; }
        public string PhoneNumber { get; private set; } = String.Empty;
        public string Email { get; private set; } = String.Empty;
        public Address Address { get; private set; } = new();
        public string Username { get; private set; } = "";
        public string PasswordHash { get; private set; } = "";
        private readonly List<Account> _accounts = [];
        public IReadOnlyCollection<Account> Accounts => _accounts.AsReadOnly();
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public bool IsActive { get; private set; }

        public Customer() { }

        public Customer(
            CustomerName name,
            DateTime dateOfBirth,
            string phoneNumber,
            string email,
            Address address,
            string username,
            string passwordHash
        )
        {
            Id = Guid.NewGuid();
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DateOfBirth = dateOfBirth;
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            Email = !string.IsNullOrWhiteSpace(email)
                ? email
                : throw new ArgumentException("Email is required");
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Username = username;
            PasswordHash = passwordHash;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public void AddAccount(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));
            _accounts.Add(account);
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required");
            if (Username == username)
                return;

            Username = username;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateName(CustomerName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePasswordHash(string newHash)
        {
            if (string.IsNullOrWhiteSpace(newHash))
                throw new ArgumentException("Password hash is required");

            PasswordHash = newHash;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateAddress(Address address)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required");

            Email = email;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePhone(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number is required");

            PhoneNumber = phoneNumber;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDateOfBirth(DateTime dob)
        {
            DateOfBirth = dob;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
