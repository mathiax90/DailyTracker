using DailyTracker.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace DailyTracker.Application.FunctionalTests;

[SetUpFixture]
public class FunctionalTestSetup
{
    internal static IServiceScopeFactory ScopeFactory { get; private set; } = null!;
    internal static DatabaseResetter? DbResetter { get; private set; }

    private static WebApiFactory? _factory;
    private static DistributedApplication? _app;

    /// <summary>
    /// Запуск один раз перед всеми тестами.
    /// Получаем подключение к БД и настраиваем необходимые статик переменные.
    /// </summary>
    /// <returns></returns>
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        var cancellationToken = cts.Token;

        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.TestAppHost>(
                args: [],
                configureBuilder: (options, _) =>
                {
                    options.DisableDashboard = true;
                });

        builder.Configuration["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true";

        _app = await builder
            .BuildAsync(cancellationToken)
        .WaitAsync(cancellationToken);

        await _app
            .StartAsync(cancellationToken)
            .WaitAsync(cancellationToken);

        await _app.ResourceNotifications.WaitForResourceHealthyAsync(
            Services.Database, cancellationToken);

        var connectionString = (await _app.GetConnectionStringAsync(Services.Database))!;

        _factory = new WebApiFactory(connectionString);
        ScopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        DbResetter = await DatabaseResetter.CreateAsync(connectionString);
    }

    /// <summary>
    /// Запуск один раз по окончании тестов.
    /// Стирает все данные из БД.
    /// </summary>
    /// <returns></returns>
    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        // 1. Сначала закрываем WebApiFactory, чтобы разорвать все активные HTTP-клиенты и пулы подключений к БД
        if (_factory is not null)
            await _factory.DisposeAsync();

        if (_app is not null)
        {
            try
            {
                // 2. Явно и мягко останавливаем контейнеры Aspire перед очисткой ресурсов
                await _app.StopAsync();
            }
            catch (Exception)
            {
                // Игнорируем исключения отмены при жестком закрытии отладчика IDE
            }
            finally
            {
                // 3. Уничтожаем сам хост Aspire
                await _app.DisposeAsync();
            }
        }

        // 4. В последнюю очередь уничтожаем ресеттер
        if (DbResetter is not null)
            await DbResetter.DisposeAsync();
    }
}
