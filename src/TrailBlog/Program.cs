using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using TrailBlog.Api.Data;
using TrailBlog.Api.Exceptions;
using TrailBlog.Api.Extensions;
using TrailBlog.Api.Repositories;
using TrailBlog.Api.Services;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicyName = "DefaultCorsPolicy";

// Cors
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsPolicyName, builder =>
    {
        builder.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowCredentials()
            .WithMethods("POST", "GET", "PUT", "PATCH", "DELETE")
            .WithExposedHeaders("X-Session-Id");

    });
});

builder.Services.AddDataProtection()
    .SetApplicationName("TrailBlog.Api");

// Add GlobalExceptionHandler
builder.Services.AddProblemDetails(configure =>
{
    configure.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
    };
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
        };
    });

// Add Rate Limitting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, token) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter = $"{retryAfter.TotalSeconds}";

            ProblemDetailsFactory problemDetailsFactory = context.HttpContext.RequestServices
                .GetRequiredService<ProblemDetailsFactory>();
            Microsoft.AspNetCore.Mvc.ProblemDetails problemDetails = problemDetailsFactory
                .CreateProblemDetails(
                    context.HttpContext,
                    StatusCodes.Status429TooManyRequests,
                    "Too Many Requests",
                    detail: $"Too many requests. Please try again after {retryAfter.TotalSeconds} seconds."
                );

            await context.HttpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: token);
        }
    };

    options.AddFixedWindowLimiter("fixed", cfg =>
    {
        cfg.PermitLimit = 20;
        cfg.Window = TimeSpan.FromMinutes(3);

    });

    options.AddPolicy("per-user", httpContext =>
    {
        string? userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrWhiteSpace(userId))
        {
            return RateLimitPartition.GetTokenBucketLimiter(
                userId,
                _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 50,
                    TokensPerPeriod = 10,
                    ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                });
        }

        return RateLimitPartition.GetFixedWindowLimiter(
            "anonymous",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromMinutes(3),
            });
    });

    options.AddPolicy("auth-operations", httpContext =>
    {
        string ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetTokenBucketLimiter(
            ipAddress,
            _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = 5,
                TokensPerPeriod = 2,
                ReplenishmentPeriod = TimeSpan.FromMinutes(1)
            });
    });
});

// Add HealthChecks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("API is running"),
        tags: new[] { "api", "live" })
    .AddNpgSql(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "postgresql-database",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "database", "ready" },
        timeout: TimeSpan.FromSeconds(5)
    );


builder.Services.AddAuthorization();

// Register repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<ICommunityRepository, CommunityRepository>();
builder.Services.AddScoped<IUserCommunityRepository, UserCommunityRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IReactionRepository, ReactionRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Register application services
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICommunityService, CommunityService>();
builder.Services.AddScoped<ICommentService, CommentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.ApplyMigrations();
}

if (app.Environment.IsProduction())
{
    var runMigrations = builder.Configuration.GetValue<bool>("RunMigrations", false);
    if (runMigrations)
    {
        app.ApplyMigrations();
    }
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseCors(CorsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseRateLimiter();

app.MapControllers();

app.Run();
