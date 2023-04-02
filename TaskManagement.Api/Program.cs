using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.JsonConverters;
using TaskManagement.Bus.Infrastructure;
using TaskManagement.Bus.Infrastructure.Services;
using TaskManagement.Bus.Services;
using TaskManagement.Services;
using TaskManagement.Services.DataContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new StatusConverter());
            });

builder.Services.AddSingleton<IRabbitConnection, RabbitConnection>();
builder.Services.AddSingleton<ServiceBusHandler>();
builder.Services.AddHostedService<ReceiveMessageBackgroundService>();
builder.Services.AddDbContext<TaskDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("TaskManagement")));

builder.Services.AddTransient<TaskManagementHandlerService>();
builder.Services.AddTransient<TaskManagementQyeryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
