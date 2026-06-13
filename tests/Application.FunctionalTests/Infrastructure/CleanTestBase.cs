using DailyTracker.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
namespace DailyTracker.Application.FunctionalTests.Infrastructure;

/// <summary>
/// Класс которые позволит наследникам запускать тесты на чистой БД
/// </summary>
public abstract class CleanTestBase
{
    /// <summary>
    /// Запуск перед каждым тестом. Перед каждым тестом стираем и наполняем.
    /// </summary>
    /// <returns></returns>
    [SetUp]
    public async Task SetUp()
    {
        //удаляем все данные
        if (FunctionalTestSetup.DbResetter != null)
        {
            await FunctionalTestSetup.DbResetter.ResetAsync();
        }

        //миграции и сид
        using (var scope = FunctionalTestSetup.ScopeFactory.CreateScope())
        {
            var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
            await initialiser.InitialiseAsync();
            await initialiser.SeedAsync();
        }
    }
}
