using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using FraudSys.Repositories;
using FraudSys.Helpers;

DatabaseManager.CreateTable(true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();
builder.Services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
