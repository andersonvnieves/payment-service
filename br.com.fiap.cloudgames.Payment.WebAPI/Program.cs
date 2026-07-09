using br.com.fiap.cloudgames.Payment.Application.Abstractions;
using br.com.fiap.cloudgames.Payment.Application.Consumers;
using br.com.fiap.cloudgames.Payment.Application.Handlers;
using br.com.fiap.cloudgames.Payment.Application.Publishers;
using br.com.fiap.cloudgames.Payment.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Payment.Application.UseCases.ApprovePayment;
using br.com.fiap.cloudgames.Payment.Application.UseCases.DeclinePayment;
using br.com.fiap.cloudgames.Payment.Domain.Repositories;
using br.com.fiap.cloudgames.Payment.Infrastructure.Config;
using br.com.fiap.cloudgames.Payment.Infrastructure.Identity;
using br.com.fiap.cloudgames.Payment.Infrastructure.Messagging;
using br.com.fiap.cloudgames.Payment.Infrastructure.Messagging.Consumers;
using br.com.fiap.cloudgames.Payment.Infrastructure.Messaging.Publishers;
using br.com.fiap.cloudgames.Payment.Infrastructure.Persistence;
using br.com.fiap.cloudgames.Payment.Infrastructure.Persistence.Context;
using br.com.fiap.cloudgames.Payment.Infrastructure.Persistence.Repositories;
using br.com.fiap.cloudgames.Payment.WebAPI;
using br.com.fiap.cloudgames.Payment.WebAPI.Middlewares;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.SingleLine = true;
    options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffzzz ";
});

//Settings
builder.Services.Configure<JwtTokenSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));

//Add Db Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")), ServiceLifetime.Scoped );

//Authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            RoleClaimType = ClaimTypes.Role
        };
    });

//Authorization
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, JwtCurrentUser>();

//Repositories
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

//UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Messaging
builder.Services.AddSingleton<RabbitMqConnection>();
builder.Services.AddSingleton<IOrderCreatedEventConsumer, OrderCreatedEventConsumer>();
builder.Services.AddScoped<IPaymentProcessedEventPublisher, PaymentProcessedEventPublisher>();

//UseCases
builder.Services.AddScoped<ApprovePaymentUseCase>();
builder.Services.AddScoped<DeclinePaymentUseCase>();

//Handlers
builder.Services.AddSingleton<OrderCreatedEventHandler>();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "PaymentAPI (FCG)",
        Version = "v1",
        Description = "Payment API "
    });
});

// Add Worker
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

//Run Migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseRequestLoggingMiddleware();
app.UseErrorHandlingMiddleware();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
