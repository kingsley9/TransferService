using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransferService.Application.DTO;
using TransferService.Application.Interfaces;
using TransferService.Domain.Entities;

namespace TransferService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCustomer(Guid id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound($"Customer {id} not found");
            return Ok(customer);
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMyProfile()
        {
            var customerIdClaim = User.FindFirstValue("customer_id");
            if (
                string.IsNullOrEmpty(customerIdClaim)
                || !Guid.TryParse(customerIdClaim, out var customerId)
            )
                return Unauthorized("Customer ID not found in claims");

            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer == null)
                return NotFound("Customer not found");
            return Ok(customer);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCustomer([FromBody] CreateCustomerRequest request)
        {
            try
            {
                var createdCustomer = await _customerService.RegisterCustomerAsync(request);
                return CreatedAtAction(
                    nameof(GetCustomer),
                    new { id = createdCustomer.Id },
                    createdCustomer
                );
            }
            catch (InvalidOperationException e)
            {
                return Conflict(new { Error = e.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCustomer(
            Guid id,
            [FromBody] UpdateCustomerRequest request
        )
        {
            var updated = await _customerService.UpdateCustomerAsync(id, request);
            if (!updated)
                return NotFound($"Customer {id} not found");
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var deleted = await _customerService.DeleteCustomerAsync(id);
            if (!deleted)
                return NotFound($"Customer {id} not found");
            return NoContent();
        }
    }
}
