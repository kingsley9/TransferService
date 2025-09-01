using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransferService.Application.Interfaces;

namespace TransferService.Application.Services
{
    public class AccountNumberGenerator : IAccountNumberGenerator
    {
        private readonly IAccountRepository _repository;
        private readonly Random _random = new();

        public AccountNumberGenerator(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> GenerateAsync(string bankCode, string schemeCode)
        {
            const int maxRetries = 20;
            var random = new Random();

            for (int i = 0; i < maxRetries; i++)
            {
                var accountNumber = random.NextInt64(1_000_000_000, 10_000_000_000).ToString();

                if (!await _repository.ExistsAsync(accountNumber))
                    return accountNumber;
            }

            throw new InvalidOperationException("Failed to generate a unique account number.");
        }
    }
}
