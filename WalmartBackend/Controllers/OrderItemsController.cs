using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalmartBackend.Data;
using WalmartBackend.Helpers;
using WalmartBackend.Models;
using WalmartBackend.Repositories;

namespace WalmartBackend.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class OrderItemsController:Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IOrderItemsRepo _orderItemsRepo;
        public OrderItemsController(ApplicationDbContext dbContext, IOrderItemsRepo orderItemsRepo)
        {
            _dbContext = dbContext;        
            _orderItemsRepo = orderItemsRepo;
        }

        [HttpPost("AddItem")]
        [Authorize]

        public async Task<ActionResult> AddItem([FromBody] OrderItems req)
        {
            try
            {
                if(req == null)
                {
                    return BadRequest(new Response(false, "Please send a valid request"));
                }
                
                var res= _orderItemsRepo.AddOrderItem(req);
                if (!res.Result.success)
                {
                    return BadRequest(res.Result);
                }

                return Ok(res.Result);
            }
            catch(Exception ex)
            {
                return Problem("Something went wrong "+ ex);
            }
        }

        [HttpGet("GetMyCart")]
        [Authorize]

        public async Task<ActionResult> GetMyCart([FromQuery]int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest(new Response(false, "Please send a valid request"));
                }
                
                return Ok(_orderItemsRepo.GetMy(id));
            }
            catch (Exception ex)
            {
                return Problem("Something went wrong " + ex);
            }
        }
    }
}
