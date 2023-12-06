using System.Text;
using api_app;
using api_app.Data;
using api_app.Data.Repository;
using api_app.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models;

var builder = WebApplication.CreateBuilder(args);


// variable to hold the CORS policy
string MyAllowedSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowedSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IMailService, MailService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var key = builder.Configuration["Jwt:Key"];
var issuer = builder.Configuration["Jwt:Issuer"];

var configuration = builder.Configuration;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<UnitOfWork>();


builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

//Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().
AddEntityFrameworkStores<ApplicationDbContext>();
// .AddDefaultTokenProviders();

var tokenValidationParameters = new TokenValidationParameters()
{
    ValidateIssuer = false, //for dev
    ValidateAudience = false, //for dev
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
    RequireExpirationTime = false, //for dev --needs to be updated when refresh token is added
    ValidateLifetime = true,
};

//Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})//JWT Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = tokenValidationParameters;
});

builder.Services.AddSingleton(tokenValidationParameters);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "App Api");
            c.InjectStylesheet("/swagger/custom.css");
            c.RoutePrefix = String.Empty;
        }
    );
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
