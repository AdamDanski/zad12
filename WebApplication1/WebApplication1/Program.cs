using WebApplication1.Data;
using WebApplication1.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Dodanie kontrolerów
builder.Services.AddControllers();

// Konfiguracja kontekstu bazy danych z użyciem "DefaultConnection"
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Rejestracja serwisów
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IClientService, ClientService>();

// Swagger i dokumentacja
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseAuthorization();

// Mapowanie kontrolerów
app.MapControllers();

// Start aplikacji
app.Run();