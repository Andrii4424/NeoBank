using Microsoft.AspNetCore.Mvc;
using Transactions.Application.DTOs;
using Transactions.Application.Filters;
using Transactions.Application.ServiceContracts;

namespace Transactions.WebUI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("{cardId}")]
        public async Task<IActionResult> GetTransactions([FromRoute] Guid cardId, [FromQuery] TransactionFilter? filters) {
            return Ok(await _transactionService.GetTransactions(cardId, filters));
        }

        [HttpGet("{cardId}/{amount}")]
        public async Task<IActionResult> GetComissionRate([FromRoute] Guid cardId)
        {
            return Ok(await _transactionService.GetComissionRate(cardId));
        }

        [HttpGet("{cardNumber}")]
        public async Task<IActionResult> GetCardId([FromRoute] string cardNumber)
        {
            Guid? result = await _transactionService.GetCardIdByCardNumberAsync(cardNumber);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> MakeTransaction([FromBody] TransactionDto transaction)
        {
            OperationResult result = await _transactionService.MakeTransaction(transaction);
            if (!result.Success) { 
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddFunds([FromBody] AddFundsDto operationDetails)
        {
            OperationResult result = await _transactionService.AddFunds(operationDetails);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok();
        }
    }
}
