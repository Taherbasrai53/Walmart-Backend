using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalmartBackend.Data;
using WalmartBackend.Helpers;
using WalmartBackend.Models;

namespace WalmartBackend.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ProductsController:Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IS3Helper _s3Helper;

        public ProductsController(ApplicationDbContext dbContext, IS3Helper s3Helper)
        {
            _dbContext = dbContext;
            _s3Helper = s3Helper;
        }

        [HttpGet("GetById")]
        [Authorize]

        public async Task<ActionResult> GetById([FromQuery] int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest(new Response(false, "please enter a valid id"));
                }

                var product = await _dbContext.Products.Where(p => p.ProductId == id).FirstOrDefaultAsync();

                return Ok(product);
            }
            catch (Exception ex)
            {
                return Problem("Something went wrong " + ex);
            }
        }

        [HttpGet("GetByName")]
        [Authorize]

        public async Task<ActionResult> GetByName([FromQuery] string name)
        {
            try
            {               
                var product = await _dbContext.Products.Where(p => p.ProductName == name).FirstOrDefaultAsync();

                return Ok(product);
            }
            catch (Exception ex)
            {
                return Problem("Something went wrong " + ex);
            }
        }
        [HttpPost("AddProduct")]
        public async Task<ActionResult> AddProduct([FromForm] AddProductModel req)
        {
            try
            {
                if(String.IsNullOrEmpty(req.ProductName))
                {
                    return BadRequest(new Response(false, "please add a valid Product name "));
                }
                if (req.ProductImage == null)
                {
                    return BadRequest(new Response(false, "please add a valid image for the profuct"));
                }

                var fileUrl = await _s3Helper.UploadFileAsync(req.ProductImage, "walmart-prods", req.ProductName);

                Console.WriteLine(req.ProductName + " " + req.ProductPrice + " " + fileUrl + " " + req.ProductInventory);
                Product prod = new Product();
                prod.ProductName = req.ProductName;
                prod.ProductImage = fileUrl;
                prod.ProductPrice = float.Parse(req.ProductPrice);
                prod.ProductInventory = int.Parse(req.ProductInventory);

                await _dbContext.Products.AddAsync(prod);

                await _dbContext.SaveChangesAsync();

                return Ok(new Response(true, "Product saved successfully"));
            }
            catch(Exception ex)
            {
                return Problem("Something went wrong "+ex);
            }
        }
    }
}
