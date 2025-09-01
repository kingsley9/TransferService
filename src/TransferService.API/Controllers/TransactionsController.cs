using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransferService.Application.DTO;
using TransferService.Application.Interfaces;
using TransferService.Domain.Exceptions;

namespace TransferService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IAccountService _accountService;

        public TransactionsController(
            ITransactionService transactionService,
            IAccountService accountService
        )
        {
            _transactionService = transactionService;
            _accountService = accountService;
        }

        [HttpPost("deposit")]
        [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deposit([FromBody] TransactionRequest request)
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
                var account = await _accountService.GetOwnedAccountAsync(
                    request.AccountId,
                    username
                );
                var result = await _transactionService.Deposit(request);
                return Ok(result);
            }
            catch (NotFoundException e)
            {
                return NotFound(new { Error = e.Message });
            }
            catch (ForbiddenException)
            {
                return Forbid();
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new { Error = e.Message });
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(new { Error = e.Message });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { Error = "An unexpected error occurred" }
                );
            }
        }

        [HttpPost("withdraw")]
        [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Withdraw([FromBody] TransactionRequest request)
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
                var account = await _accountService.GetOwnedAccountAsync(
                    request.AccountId,
                    username
                );
                var result = await _transactionService.Withdraw(request);
                return Ok(result);
            }
            catch (NotFoundException e)
            {
                return NotFound(new { Error = e.Message });
            }
            catch (ForbiddenException)
            {
                return Forbid();
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new { Error = e.Message });
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(new { Error = e.Message });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { Error = "An unexpected error occurred" }
                );
            }
        }

        [HttpPost("transfer")]
        [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Transfer([FromBody] TransactionRequest request)
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
                var account = await _accountService.GetOwnedAccountAsync(
                    request.AccountId,
                    username
                );
                var result = await _transactionService.TransferAsync(request);
                return Ok(result);
            }
            catch (NotFoundException e)
            {
                return NotFound(new { Error = e.Message });
            }
            catch (ForbiddenException)
            {
                return Forbid();
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new { Error = e.Message });
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(new { Error = e.Message });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { Error = "An unexpected error occurred" }
                );
            }
        }

        [HttpGet("{accountId}")]
        [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTransactions(int accountId)
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
                var account = await _accountService.GetOwnedAccountAsync(accountId, username);
                var history = await _transactionService.GetTransactionsForAccountAsync(accountId);
                return Ok(history);
            }
            catch (NotFoundException e)
            {
                return NotFound(new { Error = e.Message });
            }
            catch (ForbiddenException)
            {
                return Forbid();
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new { Error = e.Message });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { Error = "An unexpected error occurred" }
                );
            }
        }

        [HttpGet("{accountId}/{transactionId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTransactionDetails(int accountId, int transactionId)
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
                var account = await _accountService.GetOwnedAccountAsync(accountId, username);

                var transaction = await _transactionService.GetTransactionDetailsAsync(
                    accountId,
                    transactionId
                );
                if (transaction == null)
                    return NotFound(
                        $"Transaction {transactionId} not found for account {accountId}"
                    );
                return Ok(transaction);
            }
            catch (NotFoundException e)
            {
                return NotFound(new { Error = e.Message });
            }
            catch (ForbiddenException)
            {
                return Forbid();
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new { Error = e.Message });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { Error = "An unexpected error occurred" }
                );
            }
        }
    }
}
