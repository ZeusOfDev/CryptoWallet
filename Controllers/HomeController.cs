using CryptoWalletApp.Repositories;
using CryptoWalletApp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CryptoWalletApp.Controllers
{
    [ApiController]
    [Authorize(Roles = "basicUSer")]
    [Route("api/")]

    public class HomeController : ControllerBase
    {
        private readonly DapperRepository dprepo;
        private readonly CryptoWalletContext dbcontext;
        private readonly IConfiguration configuration;
        public HomeController(DapperRepository dprepo, CryptoWalletContext dbcontext, IConfiguration configuration) 
        {
            this.dprepo = dprepo;
            this.dbcontext = dbcontext; 
            this.configuration = configuration;
        }
        [HttpGet("/addwallet")]
        public async Task<IActionResult> AddWallet(int UserID)
        {
            return Ok(await dprepo.UserCryptoRepository.AddWallet(UserID));
        }
        [HttpGet("/walletaddress")]
        public IActionResult GetWalletAddress(int UserID)
        {
            var data = dprepo.UserCryptoRepository.GetWalletIDs(UserID);
            return Ok(data);
        }
        [HttpPost("/convert")]
        public IActionResult ConvertToUsdt(int UserID, string symbol, decimal amount)
        {
            if (amount == 0) 
                return Content("Amount can not be 0", "text/plain");
            var uc = dprepo.UserCryptoRepository.GetWalletInfoFromSymbol(symbol, UserID);
            if (uc == null)
                return NotFound("does not exist that wallet crypto user with");
            if (uc.Ucbalance < amount)
                return BadRequest("your balance is not enough to convert");
            return Ok();
        }
    }
}
