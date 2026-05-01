using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CitaCorte.API.Data.Context;
using CitaCorte.API.Service.Interfaces;
using CitaCorte.API.Service.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "default-secret-key-change-in-production";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "CitaCorte",
        ValidAudience = jwtSettings["Audience"] ?? "CitaCorteUsers",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IBarberoService, BarberoService>();
builder.Services.AddScoped<IBarberiaService, BarberiaService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IComercialService, ComercialService>();
builder.Services.AddScoped<IClientService, ClientService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CitaCorte API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .WithOrigins("http://localhost:5173", "http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Initialize database on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        context.Database.EnsureCreated();
        SeedData(context);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

app.Run();

static void SeedData(ApplicationDbContext context)
{
    // Seed default subscription plans if they don't exist
    if (!context.SubscriptionPlans.Any())
    {
        context.SubscriptionPlans.AddRange(
            // Barbero Plans
            new SubscriptionPlan
            {
                Type = SubscriptionType.Free,
                Name = "Free",
                Description = "Plan gratuito para barberos. Solo permite mostrar perfil y servicios.",
                IsForBarbero = true,
                IsForBarberia = false,
                Price = 0,
                DurationDays = 0,
                CanReceiveReservations = false,
                CanViewStatistics = false,
                CanSellProducts = false,
                CanEditProfile = true,
                CanViewServices = true
            },
            new SubscriptionPlan
            {
                Type = SubscriptionType.Popular,
                Name = "Popular",
                Description = "Plan popular para barberos. Permite recibir reservas y ver estadísticas básicas.",
                IsForBarbero = true,
                IsForBarberia = false,
                Price = 9.99m,
                Currency = "USD",
                DurationDays = 30,
                CanReceiveReservations = true,
                CanViewStatistics = true,
                CanSellProducts = false,
                CanEditProfile = true,
                CanViewServices = true
            },
            new SubscriptionPlan
            {
                Type = SubscriptionType.Premium,
                Name = "Premium",
                Description = "Plan premium para barberos. Incluye todas las funcionalidades más venta de productos.",
                IsForBarbero = true,
                IsForBarberia = false,
                Price = 19.99m,
                Currency = "USD",
                DurationDays = 30,
                CanReceiveReservations = true,
                CanViewStatistics = true,
                CanSellProducts = true,
                CanEditProfile = true,
                CanViewServices = true
            },
            // Barberia Plans
            new SubscriptionPlan
            {
                Type = SubscriptionType.Popular,
                Name = "Barbería Popular",
                Description = "Plan popular para barberías. Hasta 5 barberos afiliados.",
                IsForBarbero = false,
                IsForBarberia = true,
                Price = 29.99m,
                Currency = "USD",
                DurationDays = 30,
                CanReceiveReservations = true,
                CanViewStatistics = true,
                CanSellProducts = false,
                CanEditProfile = true,
                CanViewServices = true,
                MaxBarberosLimit = 5
            },
            new SubscriptionPlan
            {
                Type = SubscriptionType.Premium,
                Name = "Barbería Premium",
                Description = "Plan premium para barberías. Hasta 15 barberos afiliados.",
                IsForBarbero = false,
                IsForBarberia = true,
                Price = 59.99m,
                Currency = "USD",
                DurationDays = 30,
                CanReceiveReservations = true,
                CanViewStatistics = true,
                CanSellProducts = false,
                CanEditProfile = true,
                CanViewServices = true,
                MaxBarberosLimit = 15
            }
        );
        context.SaveChanges();
    }

    // Seed admin user if doesn't exist
    if (!context.Users.Any(u => u.Role == UserRole.Admin))
    {
        context.Users.Add(new User
        {
            Name = "Administrador",
            Email = "admin@citacorte.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = UserRole.Admin,
            IsActive = true
        });
        context.SaveChanges();
    }

    // Seed comercial user if doesn't exist
    if (!context.Users.Any(u => u.Role == UserRole.Comercial))
    {
        context.Users.Add(new User
        {
            Name = "Comercial",
            Email = "comercial@citacorte.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Comercial123!"),
            Role = UserRole.Comercial,
            IsActive = true
        });
        context.SaveChanges();
    }
}
