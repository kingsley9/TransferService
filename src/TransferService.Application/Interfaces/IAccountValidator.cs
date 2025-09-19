using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransferService.Domain.Rules;

namespace TransferService.Application.Interfaces
{
    public interface IAccountValidator
    {
        public void Validate(AccountValidationContext context);
    }
}
