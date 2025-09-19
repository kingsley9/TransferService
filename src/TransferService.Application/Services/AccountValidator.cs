using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransferService.Application.Interfaces;
using TransferService.Domain.Rules;

namespace TransferService.Application.Services
{
    public class AccountValidator : IAccountValidator
    {
        private readonly IEnumerable<IAccountRule> _rules;

        public AccountValidator(IEnumerable<IAccountRule> rules)
        {
            _rules = rules;
        }

        public void Validate(AccountValidationContext context)
        {
            var applicableRules = _rules.Where(r =>
                (!r.AccountType.HasValue || r.AccountType == context.Account.Type)
                && (!r.Tier.HasValue || r.Tier == context.Account.Tier)
                && (String.IsNullOrEmpty(r.Currency) || r.Currency == context.Account.Currency)
            );

            foreach (var rule in applicableRules)
            {
                rule.Validate(context);
            }
        }
    }
}
