using backend.Data;
using backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// Load Environment Variables
// ==========================

var server = Environment.GetEnvironmentVariable("DB_SERVER");
var db = Environment.GetEnvironmentVariable("DB_NAME");
var user = Environment.GetEnvironmentVariable("DB_USER");
var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

Console.WriteLine($"DB_SERVER={server}");
Console.WriteLine($"DB_NAME={db}");
Console.WriteLine($"DB_USER={user}");

var connectionString =
$"Server={server},1433;Database={db};User Id={user};Password={password};TrustServerCertificate=True;Encrypt=False;";

// ==========================
// Database
// ==========================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// ==========================
// Controllers + Swagger
// ==========================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ==========================
// Dependency Injection
// ==========================

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<HashService>();
builder.Services.AddScoped<SignatureService>();
builder.Services.AddScoped<QrService>();
builder.Services.AddScoped<VerifyService>();

// ==========================
// CORS
// ==========================

builder.Services.AddCors(options =>
{
    options.AddPolicy("allow",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


builder.WebHost.UseUrls("http://0.0.0.0:5299");
// ==========================
// BUILD APP
// ==========================

var app = builder.Build();


// ==========================
// Create DB Automatically
// ==========================

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var retries = 10;

    while (true)
    {
        try
        {
            context.Database.EnsureCreated();
            Console.WriteLine("Database connected successfully.");
            break;
        }
        catch (Exception ex)
        {
            retries--;

            Console.WriteLine($"DB not ready yet. Retries left: {retries}");
            Console.WriteLine(ex.Message);

            if (retries == 0)
            {
                throw; // fail only after multiple attempts
            }

            Thread.Sleep(5000); // wait 5 seconds
        }
    }
}

// ==========================
// Middleware
// ==========================

app.UseCors("allow");

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();