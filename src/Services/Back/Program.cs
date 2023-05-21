using Back.Hubs;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddSignalR(opt => { opt.EnableDetailedErrors = true; });

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseAuthorization();

app.UseCors();

app.MapControllers();
app.MapHub<ChatHub>("/chat");

app.Run();