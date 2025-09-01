using System.ComponentModel.DataAnnotations;

namespace TransferService.Application.DTO
{
    public class UpdateCustomerRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
    }
}
