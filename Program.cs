using Tasks.Services;
using ToDo.Middlewares;
using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;
            cfg.TokenValidationParameters = TasksTokenService.GetTokenValidationParameters();
        });

builder.Services.AddAuthorization(cfg =>
   {
       cfg.AddPolicy("Admin", policy => policy.RequireClaim("type","Admin"));
       cfg.AddPolicy("User", policy => policy.RequireClaim("type", "User","Admin"));
   });

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
   c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tasks", Version = "v1" });
   c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
   {
       In = ParameterLocation.Header,
       Description = "Please enter JWT with Bearer into field",
       Name = "Authorization",
       Type = SecuritySchemeType.ApiKey
   });
   c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                { new OpenApiSecurityScheme
                        {
                         Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer"}
                        },
                    new string[] {}
                }
   });
});

builder.Services.AddControllers();
builder.Services.AddTask();
builder.Services.AddUser();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UselogMiddleware("file.txt");
//app.UseTokenExpMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// builder. builder.Services.AddScoped<ITaskServices, TaskServices>();

app.UseDefaultFiles();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
