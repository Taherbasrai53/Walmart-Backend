using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using WalmartBackend.Data;
using System.Text;
using WalmartBackend.Helpers;
using System.Text.Json.Serialization;
using Amazon.S3;
using Amazon.Extensions.NETCore.Setup;
using WalmartBackend.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

// Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StagingConnection")));

// JWT Authentication Setup
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = async (context) =>
            {
                Console.WriteLine("Printing in the delegate OnAuthFailed");
            },
            OnChallenge = async (context) =>
            {
                context.HandleResponse();

                if (context.AuthenticateFailure != null)
                {
                    context.Response.StatusCode = 401;
                    var res = new { success = false, Message = "Unauthorized Access, Access Denied" };
                    await context.HttpContext.Response.WriteAsJsonAsync(res);
                }
            }
        };
    });

// Kestrel Configuration
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "80";
    serverOptions.ListenAnyIP(int.Parse(port));
});

// CORS Setup
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Dependency Injection
builder.Services.AddTransient<ICommonHelper, CommonHelper>();
builder.Services.AddTransient<IS3Helper, S3Helper>();
builder.Services.AddTransient<ITrollyRepo, TrollyRepo>();
builder.Services.AddTransient<ISessionRepo, SessionRepo>(); 
builder.Services.AddTransient<IOrderItemsRepo, OrderItemsRepo>();

// JSON Serialization Setup
builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Swagger Setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AWS Services Setup
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());

var app = builder.Build();

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Urls.Add("http://*:5000");

app.Run();
