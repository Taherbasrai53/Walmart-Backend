using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WalmartBackend.Data;
using WalmartBackend.Helpers;
using WalmartBackend.Models;
using WalmartBackend.Repositories;

namespace WalmartBackend.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class SessionsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ISessionRepo _sessionRepo;
        public SessionsController(ApplicationDbContext dbContext, ISessionRepo sessionRepo)
        {
            _dbContext = dbContext;
            _sessionRepo = sessionRepo; 
        }

        [HttpGet("Check")]
        [Authorize]
        public async Task<ActionResult> CheckSession([FromQuery] int orderId)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var claimUserId = claimsIdentity.FindFirst("userId")?.Value;
                var TokenUserId = 0;

                int.TryParse(claimUserId, out TokenUserId);

                return Ok(_sessionRepo.Check(orderId, TokenUserId));
            }
            catch (Exception ex)
            {
                return Problem("Something went wrong");
            }
        }

        [HttpPost("End")]
        [Authorize]

        public async Task<ActionResult> Endsession([FromBody] EndSessionModel req)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var claimUserId = claimsIdentity.FindFirst("userId")?.Value;
                var TokenUserId = 0;

                int.TryParse(claimUserId, out TokenUserId);

                return Ok(_sessionRepo.EndSession(TokenUserId, req));
            }
            catch (Exception ex)
            {
                return Problem("Something went wrong");
            }
        }
    }
}
