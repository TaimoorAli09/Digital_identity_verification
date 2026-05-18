using backend.Data;
using backend.Services;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// ==========================Loading environmental variables from .env =====================================
Env.Load();

var server = Environment.GetEnvironmentVariable("DB_SERVER");

var db = Environment.GetEnvironmentVariable("DB_NAME");

var user = Environment.GetEnvironmentVariable("DB_USER");

var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionString =
$"Server={server};Database={db};User Id={user};Password={password};TrustServerCertificate=True;";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
// =========================================================================================
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();


// ========== Adding Services from services folder ==============

// these AddScoped are dependency injection 

builder.Services.AddScoped<TokenService>();

builder.Services.AddScoped<HashService>();

builder.Services.AddScoped<SignatureService>();

builder.Services.AddScoped<QrService>();

builder.Services.AddScoped<VerifyService>();

//====================== Adding cors for  connecting frontend api ========================

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


var app = builder.Build();
app.UseCors("allow");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();