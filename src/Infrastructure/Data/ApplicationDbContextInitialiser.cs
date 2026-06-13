using DailyTracker.Domain.Constants;
using DailyTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DailyTracker.Infrastructure.Data;

public static class InitialiserExtensions
{
    /// <summary>
    /// Пересоздаёт БД (через миграции или напрямую из моделей) и вызывает seed.
    /// Использовать только для dev и staging
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static async Task InitialiseAndSeedDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }

    /// <summary>
    /// Вызывает seed существующей БД.
    /// Используется для prod.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static async Task SeedDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _env;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context, 
        UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _env = environment;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            // See https://jasontaylor.dev/ef-core-database-initialisation-strategies
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedProdAsync();
            if (_env.IsDevelopment())
            {
                await TrySeedDevAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    /// <summary>
    /// Сеет прод данные
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task TrySeedProdAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole(Roles.Administrator);

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        // Default users
        var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            string adminPassword = _configuration["AdminSettings:Password"] ?? "LocalDevDefault1!@";
            // 1. Сохраняем результат создания пользователя
            var result = await _userManager.CreateAsync(administrator, adminPassword);

            // 2. Проверяем, что запись в AspNetUsers прошла успешно
            if (result.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(administratorRole.Name))
                {
                    await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
                }
            }
            else
            {
                // 3. Если Identity отклонил пароль или данные, выводим точную причину в логи
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Ошибка инициализации админа. Identity отказал по причине: {errors}");
            }
        }
    }

    /// <summary>
    /// Сеет дев данные
    /// </summary>
    /// <returns></returns>
    public async Task TrySeedDevAsync()
    {
        await Task.CompletedTask;
    }
}
