using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransferService.Domain.Entities;

namespace TransferService.Domain.Rules
{
    public class AccountValidationContext
    {
        public Account Account { get; set; } = new();
        public decimal? DepositAmount { get; set; }
        public decimal? WithdrawalAmount { get; set; }
    }
}
