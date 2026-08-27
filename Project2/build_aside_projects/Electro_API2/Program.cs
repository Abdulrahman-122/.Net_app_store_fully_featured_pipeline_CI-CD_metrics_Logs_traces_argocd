using ElectroAPI.Data;
using ElectroAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ElectroDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(10, 4, 0))
    ));
// builder.Services.AddHealthChecks().AddDbContextCheck<ElectroDbContext>();
builder.Services.AddHealthChecks().AddCheck("self", () =>   HealthCheckResult.Healthy()).AddDbContextCheck<ElectroDbContext>("Database").ForwardToPrometheus();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<ShipmentService>();
// builder.Services.AddScoped<ReviewService>();
// builder.Services.AddScoped<SupplierService>();
// builder.Services.AddScoped<StockTransactionService>();

builder.Services.AddScoped<AdminService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Electro API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your_token_here}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHostedService<UserMetricsCollector>();
builder.Services.AddHostedService<OrderMetricsCollector>();

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://*:{port}");
}
else
{
    // When running locally and no PORT provided, listen on all interfaces so LAN devices can connect.
    var defaultPort = Environment.GetEnvironmentVariable("ASPNETCORE_PORT") ?? "5000";
    builder.WebHost.UseUrls($"http://0.0.0.0:{defaultPort}");
}

var app = builder.Build();

// Apply EF Core migrations at startup so the Railway MySQL schema is created/updated.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ElectroDbContext>();
    db.Database.Migrate();
}

// Enable static files
app.UseStaticFiles();
// for  promtheus
app.UseHttpMetrics();
app.MapControllers();
app.MapMetrics();

// Only use HTTPS redirect in production
// if (!app.Environment.IsDevelopment())
// {
//     app.UseHttpsRedirection();
// } 

// Redirect root "/" to login.html
app.MapGet("/", context =>
{
    context.Response.Redirect("/login.html");
    return Task.CompletedTask;
});

// Swagger only at /swagger
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Electro API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();
// app.MapHealthChecks("/health");
// app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live",new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate =check => check.Name == "self"
});
app.MapHealthChecks("/health/ready",new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Name =="Database"
});



app.Run();
