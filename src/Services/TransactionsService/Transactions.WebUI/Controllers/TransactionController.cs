using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Transactions.Application.DTOs;
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

        [HttpPost]
        public async Task<IActionResult> MakeTransaction([FromBody] TransactionDto transaction)
        {
            OperationResult result = await _transactionService.MakeTransaction(transaction);
            if (!result.Success) { 
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }
    }
}
