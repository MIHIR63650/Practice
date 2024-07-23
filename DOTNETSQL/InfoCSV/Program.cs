using Microsoft.OpenApi.Models;
using MySql.Data.MySqlClient;
using MongoDB.Driver;
using Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Logging.AddConsole(); // Add console logging

// Retrieve connection strings
string? mySqlConnectionStr = builder.Configuration.GetConnectionString("DefaultConnection");
string? mongoDbConnectionStr = builder.Configuration.GetConnectionString("MongoDbConnection");
string? mongoDbDatabaseName = builder.Configuration.GetSection("MongoDbSettings:DatabaseName").Value;

// Check for null MySQL connection string
if (mySqlConnectionStr == null)
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
}

// Check for null MongoDB connection string and database name
if (mongoDbConnectionStr == null)
{
    throw new InvalidOperationException("Connection string 'MongoDbConnection' not found in configuration.");
}

if (mongoDbDatabaseName == null)
{
    throw new InvalidOperationException("MongoDB database name not found in configuration.");
}

// Register MySQL as a singleton connection
builder.Services.AddSingleton(new MySqlConnection(mySqlConnectionStr));

// Register MongoDB services
builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoDbConnectionStr));
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoDbDatabaseName);
});

// Register FileStateService as a scoped service
builder.Services.AddScoped<FileStateService>();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

// Configure app
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Middleware pipeline
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

// Run the app
app.Run();
