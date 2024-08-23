using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WalmartBackend.Models;
using WalmartBackend.Repositories;

namespace WalmartBackend.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]

    public class TrolliesController:Controller
    {
        ITrollyRepo _trollyRepo;
        public TrolliesController(ITrollyRepo trollyRepo)
        {
            _trollyRepo = trollyRepo;
        }

        [HttpGet("Scan")]
        [Authorize]

        public async Task<ActionResult> Scan([FromQuery] int id)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var claimUserId = claimsIdentity.FindFirst("userId")?.Value;
                var TokenUserId = 0;
                
                int.TryParse(claimUserId, out TokenUserId);

                Console.WriteLine(TokenUserId+" "+ id);
                var res = _trollyRepo.ScanTrolly(id, TokenUserId);
                if (res.Result is Response)
                {
                    return BadRequest(res.Result);
                }

                return Ok(res);
            }
            catch (Exception ex)
            {
                return Problem("Something went wrong " + ex);
            }
        }
    }
}
