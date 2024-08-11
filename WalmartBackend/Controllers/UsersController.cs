using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WalmartBackend.Data;
using WalmartBackend.Models;
using Response = WalmartBackend.Models.Response;
using LoginResponse = WalmartBackend.Models.LoginResponse;

namespace WalmartBackend.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class UsersController:Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _config;

        public UsersController(ApplicationDbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }

        private string GenerateToken(User user)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                 {
                    new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("userId", user.UserId.ToString())                    
                 };

                var token = new JwtSecurityToken
                    (
                    _config["Jwt:Issuer"],
                    _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddYears(12),
                    signingCredentials: credentials

                    );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        [AllowAnonymous]
        [HttpPost("Login")]

        public async Task<ActionResult> Login(LoginModel req)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(req.UserName))
                { return BadRequest(new Response(false, "Please enter a valid userId")); }

                if (string.IsNullOrWhiteSpace(req.Password))
                {
                    return BadRequest(new Response(false, "Please enter a valid password"));
                }

                var user = await _dbContext.Users.Where(u => u.UserName == req.UserName && u.Password == req.Password).FirstOrDefaultAsync();

                if(user==null)
                {
                    return BadRequest(new Response(false, "Invaid Credentials"));
                }

                var token = GenerateToken(user);
                return Ok(new LoginResponse(true, token));

            }
            catch (Exception ex)
            {
                return Problem("Something went wrong");
            }
        }


        [AllowAnonymous]
        [HttpPost("SignUp")]

        public async Task<ActionResult> Signup(User user)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(user.UserName))
                { return BadRequest(new Response(false, "Please enter a valid userId")); }

                if (string.IsNullOrWhiteSpace(user.Password))
                {
                    return BadRequest(new Response(false, "Please enter a valid password"));
                }

                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    return BadRequest(new Response(false, "Please enter a valid email"));
                }

                var newUser = await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();

                return Ok(new Response(true, "User Added Successfully"));
            }
            catch (Exception ex)
            {
                return Problem("Oops something went wrong");
            }
        }
    }
}
