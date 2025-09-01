using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferService.Application.Interfaces
{
    public interface IAccountNumberGenerator
    {
        Task<string> GenerateAsync(string bankCode, string schemeCode);
    }
}
