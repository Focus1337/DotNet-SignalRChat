using Back.Web;
using Back.Web.Hubs;
using Back.Web.Hubs.Filters;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

builder.AddCustomControllers();

builder.AddCustomDb();
builder.AddCustomIdentity();
builder.ConfigureCustomIdentityOptions();
builder.AddCustomOpenIddict();

services.AddSignalR(opt =>
{
    opt.EnableDetailedErrors = true;
    opt.AddFilter<CustomFilter>();
});

services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost", "http://localhost:3000")
            .AllowAnyHeader()
            .WithMethods("GET", "POST", "PATCH")
            .AllowCredentials();
    });
});

services.AddEndpointsApiExplorer();

builder.AddCustomSwaggerGen();

builder.AddCustomAuthentication();
services.AddAuthorization();

builder.AddCustomApplicationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.MigrateDbContext();

app.UseCors();

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chat");

app.Run();