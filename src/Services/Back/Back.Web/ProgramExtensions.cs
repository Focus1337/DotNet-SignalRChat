using Back.Application.Interfaces;
using Back.Application.Models;
using Back.Infrastructure.Data;
using Back.Infrastructure.Options;
using Back.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenIddict.Abstractions;
using Polly;

namespace Back.Web;

public static class ProgramExtensions
{
    private const string AppName = "SignalRChat";

    public static void AddCustomControllers(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });
        builder.Services.AddControllers()
            // .ConfigureApiBehaviorOptions(options =>
            // {
            //     options.InvalidModelStateResponseFactory = context =>
            //     {
            //         var errors = context.ModelState.Values.SelectMany(x => x.Errors.Select(p => p.ErrorMessage))
            //             .ToList();
            //         return new BadRequestObjectResult(new ModelStateDto { Errors = errors });
            //     };
            // })
            .AddNewtonsoftJson();
    }

    public static void AddCustomApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IUserService<User>, UserService>();
        // builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    public static void AddCustomSwaggerGen(this WebApplicationBuilder builder) =>
        builder.Services.AddSwaggerGen(option =>
        {
            option.EnableAnnotations();
            option.SwaggerDoc("v1", new OpenApiInfo { Title = $"{AppName} API", Version = "v1" });
            option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Password = new OpenApiOAuthFlow
                    {
                        TokenUrl = new Uri("/connect/token", UriKind.Relative)
                    }
                }
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

    public static void AddCustomDb(this WebApplicationBuilder builder)
    {
        builder.Services
            .Configure<DbOptions>(
                builder.Configuration.GetSection(DbOptions.DbConfiguration));

        var dbOptions = builder.Configuration.GetSection(DbOptions.DbConfiguration).Get<DbOptions>();

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(dbOptions!.ConnectionString,
                action => action.MigrationsAssembly(typeof(AppDbContext).Assembly
                    .FullName));
            options.UseOpenIddict();
            options.EnableSensitiveDataLogging();
        });
    }

    public static void AddCustomAuthentication(this WebApplicationBuilder builder) =>
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => options.ClaimsIssuer = JwtBearerDefaults.AuthenticationScheme)
            .AddCookie(options => { options.LoginPath = new PathString("/connect/token"); });

    public static void AddCustomOpenIddict(this WebApplicationBuilder builder) =>
        builder.Services.AddOpenIddict()
            .AddCore(options =>
                options.UseEntityFrameworkCore()
                    .UseDbContext<AppDbContext>())
            .AddServer(options =>
            {
                options
                    .AcceptAnonymousClients()
                    .AllowPasswordFlow()
                    .AllowRefreshTokenFlow();
                options.SetTokenEndpointUris("/connect/token");
                var cfg = options.UseAspNetCore();
                if (builder.Environment.IsDevelopment())
                    cfg.DisableTransportSecurityRequirement();
                cfg.EnableTokenEndpointPassthrough();
                options
                    .AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();
            })
            .AddValidation(options =>
            {
                options.UseAspNetCore();
                options.UseLocalServer();
            });

    public static void AddCustomIdentity(this WebApplicationBuilder builder) =>
        builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

    public static void ConfigureCustomIdentityOptions(this WebApplicationBuilder builder) =>
        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
            options.ClaimsIdentity.EmailClaimType = OpenIddictConstants.Claims.Email;
            options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Username;
        });

    public static async Task MigrateDbContext(this WebApplication app) =>
        await Policy.Handle<Exception>()
            .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(10),
                onRetry: (exception, retryTime) =>
                    Console.WriteLine($"Error on migration apply: {exception.Message} | Retry in {retryTime}"))
            .ExecuteAsync(async () =>
            {
                await using var scope = app.Services.CreateAsyncScope();
                var context = scope.ServiceProvider.GetService<AppDbContext>();
                await context!.Database.MigrateAsync();
            });
}