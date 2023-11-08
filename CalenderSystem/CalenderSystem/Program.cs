using CalenderSystem.Application;
using CalenderSystem.Domain.Entities.Identity;
using CalenderSystem.Infrastructure;
using CalenderSystem.Infrastructure.Database;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        // ignore null values when serializing to json
        opt.JsonSerializerOptions.DefaultIgnoreCondition
                       = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddSwaggerGen(options =>
{
    // ...
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter your access token in the field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
//Adding the DbContext 
builder.Services.AddDbContext<AppDbContext>(opt =>
				opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
// adding identity services like user manager, role manager etc.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
//add google auth
builder.Services.AddAuthentication()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:client_id"];
    options.ClientSecret = builder.Configuration["Authentication:Google:client_secret"];
});
//
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructureDependencies()
	.AddApplicationDependeicies();
//allow the cors for my blazor app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder
            .WithOrigins("https://localhost:7191") // Replace with the actual origin of your Blazor WebAssembly app
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
