using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Transactions.WebUI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Test()
        {
            return Ok();
        }

    }
}
