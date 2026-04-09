using Microsoft.EntityFrameworkCore;
using Printawyapis.Data;
var builder = WebApplication.CreateBuilder(args);


// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular dev server
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add controllers
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
// Add Swagger services
builder.Services.AddEndpointsApiExplorer(); // Needed for minimal APIs
builder.Services.AddSwaggerGen(); // Generates Swagger docs

var app = builder.Build();

// Use CORS
app.UseCors("AllowAngular");

// Enable Swagger middleware (only in development, optional)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Generates Swagger JSON
    app.UseSwaggerUI(); // Serves the Swagger UI at /swagger
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();