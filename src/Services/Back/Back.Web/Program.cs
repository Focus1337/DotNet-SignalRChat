using Back.Web;
using Back.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

builder.AddCustomControllers();

builder.AddCustomDb();
builder.AddCustomIdentity();
builder.ConfigureCustomIdentityOptions();
builder.AddCustomOpenIddict();

services.AddSignalR(opt => { opt.EnableDetailedErrors = true; });

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

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chat");

app.Run();