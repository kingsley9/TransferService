using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransferService.Domain.Entities;

namespace TransferService.Application.Interfaces
{
    public interface IPinService
    {
        void SetPin(Account account, string rawPin);
        void ChangePin(Account account, string currentPin, string newPin);
        bool VerifyPin(Account account, string rawPin);
    }
}
