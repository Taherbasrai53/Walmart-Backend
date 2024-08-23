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

        [HttpPut("Update")]
        [Authorize]
        public async Task<ActionResult> UpdateItem([FromBody] OrderItems req)
        {
            try
            {
                return Ok(_orderItemsRepo.UpdateOrderItem(req));
            }
            catch (Exception ex) { return Problem("something went wrong"); }
        }

        [HttpDelete("Delete")]
        [Authorize]
        public async Task<ActionResult> DeleteItem([FromQuery] int id)
        {
            try
            {
                return Ok(_orderItemsRepo.DeleteOrderItem(id));
            }
            catch (Exception ex) { return Problem("something went wrong"); }
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
