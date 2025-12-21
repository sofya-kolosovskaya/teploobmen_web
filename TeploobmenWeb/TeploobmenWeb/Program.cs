// Program.cs
using Microsoft.EntityFrameworkCore;
using HeatExchangeApp.Data;
using HeatExchangeApp.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Регистрация сервисов ДО builder.Build()

// MVC
builder.Services.AddControllersWithViews();

// Сессии (добавляем ДО других сервисов, которые могут их использовать)
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
builder.Services.AddScoped<AuthService>();  // Если используете авторизацию

// HttpContextAccessor для доступа к HttpContext в сервисах
builder.Services.AddHttpContextAccessor();

// 2. Сборка приложения
var app = builder.Build();

// 3. Конфигурация middleware ПОСЛЕ builder.Build()

// Настройка конвейера middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Маршрутизация должна быть ДО авторизации
app.UseRouting();

// Сессии ДО авторизации
app.UseSession();

app.UseAuthorization();

// Создание БД при запуске (если не существует)
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