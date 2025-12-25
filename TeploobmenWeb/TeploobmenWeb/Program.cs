
using Microsoft.EntityFrameworkCore;
using HeatExchangeApp.Data;
using HeatExchangeApp.Services;

var builder = WebApplication.CreateBuilder(args);


// MVC
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "HeatExchange.Session";
});

// База данных SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=calculations.db"));

// Регистрация собственных сервисов
builder.Services.AddScoped<CalculationService>();
builder.Services.AddScoped<AuthService>();

// HttpContextAccessor для доступа к HttpContext в сервисах
builder.Services.AddHttpContextAccessor();

// 2. Сборка приложения
var app = builder.Build();


// Настройка конвейера middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();


using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
        Console.WriteLine("База данных создана/подключена успешно");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при создании БД: {ex.Message}");
    }
}

// Маршруты
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");  

app.Run();