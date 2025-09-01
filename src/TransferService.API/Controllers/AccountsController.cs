using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransferService.Application.DTO;
using TransferService.Application.Interfaces;
using TransferService.Domain.Entities;
using TransferService.Domain.Exceptions;

namespace TransferService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAccount(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null)
                return NotFound($"Account {id} not found");
            return Ok(account);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAccount([FromBody] AccountRequest account)
        {
            var username = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var ownerName = User.FindFirstValue("full_name") ?? "";
            var customerId = Guid.Parse(User.FindFirstValue("customer_id") ?? "");
            var created = await _accountService.CreateAccountAsync(
                account,
                ownerName,
                username,
                customerId
            );
            Console.WriteLine(created.ToString());
            return CreatedAtAction(nameof(GetAccount), new { id = created.AccountId }, created);
        }

        // [HttpPut("{id}")]
        // public async Task<IActionResult> UpdateAccount(int id, [FromBody] Account account)
        // {
        //     if (id != account.AccountId)
        //         return BadRequest("Account ID mismatch");

        //     var updated = await _accountService.UpdateAccountAsync(account);
        //     if (!updated)
        //         return NotFound($"Account {id} not found");

        //     return NoContent();
        // }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Account), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var deleted = await _accountService.DeleteAccountAsync(id);
            if (!deleted)
                return NotFound($"Account {id} not found");
            return NoContent();
        }

        // [HttpGet]
        // public async Task<IActionResult> GetAllAccounts()
        // {
        //     var accounts = await _accountService.GetAllAccountsAsync();
        //     return Ok(accounts);
        // }

        [HttpGet("{id}/balance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBalance(int id)
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
                var account = await _accountService.GetOwnedAccountAsync(id, username);
                if (account == null)
                    return NotFound($"Account {id} not found");
                var balance = account.Balance;
                return Ok(new { AccountId = id, Balance = balance });
            }
            catch (NotFoundException e)
            {
                return NotFound(new { Error = e.Message });
            }
            catch (ForbiddenException)
            {
                return Forbid();
            }
        }
    }
}
