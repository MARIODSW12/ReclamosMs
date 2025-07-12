using DotNetEnv;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using System.Reflection;

using Reclamos.Application.Handlers;
using Reclamos.Application.Validations;

using Reclamos.Domain.Events;
using Reclamos.Domain.Repositories;

using Reclamos.Infrastructure.Configurations;
using Reclamos.Infrastructure.Consumer;
using Reclamos.Infrastructure.Interfaces;
using Reclamos.Infrastructure.Persistences.Repositories.MongoRead;
using Reclamos.Infrastructure.Persistences.Repositories.MongoWrite;
using Reclamos.Infrastructure.Services;
using Reclamos.Infrastructure.Queries.QueryHandler;
using RestSharp;


var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<IRestClient>(new RestClient());

// Registrar configuración de MongoDB
builder.Services.AddSingleton<MongoWriteReclamoDbConfig>();
builder.Services.AddSingleton<MongoReadReclamoDbConfig>();

// REGISTRA EL REPOSITORIO ANTES DE MediatR
builder.Services.AddScoped<IClaimRepository, ClaimWriteRepository>();
builder.Services.AddScoped<IClaimReadRepository, ClaimReadRepository>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

// REGISTRA MediatR PARA TODOS LOS HANDLERS
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateClaimCommandHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SolveClaimCommandHandler).Assembly));


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetClaimPorIdQueryHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetTodosLosClaimPorStatusQueryHandler).Assembly));


builder.Services.AddValidatorsFromAssemblyContaining<CreateClaimDtoValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<SolveClaimDtoValidation>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<CreateClaimConsumer>();
    busConfigurator.AddConsumer<SolveClaimConsumer>();
    busConfigurator.AddConsumer<OpenClaimConsumer>();
    busConfigurator.AddConsumer<SendNotificationConsumer>();

    busConfigurator.SetKebabCaseEndpointNameFormatter();
    busConfigurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(new Uri(Environment.GetEnvironmentVariable("RABBIT_URL")), h =>
        {
            h.Username(Environment.GetEnvironmentVariable("RABBIT_USERNAME"));
            h.Password(Environment.GetEnvironmentVariable("RABBIT_PASSWORD"));
        });

        configurator.ReceiveEndpoint(Environment.GetEnvironmentVariable("RABBIT_QUEUE_CLAIM"), e => {
            e.ConfigureConsumer<CreateClaimConsumer>(context);
        });
        configurator.ReceiveEndpoint(Environment.GetEnvironmentVariable("RABBIT_QUEUE_SOLVE_CLAIM"), e => {
            e.ConfigureConsumer<SolveClaimConsumer>(context);
        });
        configurator.ReceiveEndpoint(Environment.GetEnvironmentVariable("RABBIT_QUEUE_OPEN_CLAIM"), e => {
            e.ConfigureConsumer<OpenClaimConsumer>(context);
        });
        configurator.ReceiveEndpoint(Environment.GetEnvironmentVariable("RABBIT_QUEUE_ENVIAR_NOTIFICACION"), e => {
            e.ConfigureConsumer<SendNotificationConsumer>(context);
        });


        configurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        configurator.ConfigureEndpoints(context);
    });
});
EndpointConvention.Map<ClaimCreatedEvent>(new Uri("queue:" + Environment.GetEnvironmentVariable("RABBIT_QUEUE_CLAIM")));
EndpointConvention.Map<ClaimSolvedEvent>(new Uri("queue:" + Environment.GetEnvironmentVariable("RABBIT_QUEUE_SOLVE_CLAIM")));
EndpointConvention.Map<ClaimOpenedEvent>(new Uri("queue:" + Environment.GetEnvironmentVariable("RABBIT_QUEUE_OPEN_CLAIM")));
EndpointConvention.Map<NotificationSendEvent>(new Uri("queue:" + Environment.GetEnvironmentVariable("RABBIT_QUEUE_ENVIAR_NOTIFICACION")));

// Configuración CORS permisiva (¡Solo para desarrollo!)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Permite cualquier dominio
            .AllowAnyMethod()  // GET, POST, PUT, DELETE, etc.
            .AllowAnyHeader(); // Cualquier cabecera
    });
});

var app = builder.Build();

// Habilitar CORS
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
