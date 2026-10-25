using System.Globalization;
using DailyTracker.Domain.Constants;
using DailyTracker.Domain.Entities;
using DailyTracker.Domain.Entities.Activities;
using DailyTracker.Domain.Enums;
using DailyTracker.Domain.Exceptions;
using DailyTracker.Domain.Repositories;
using DailyTracker.Domain.ValueObjects;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace DailyTracker.Domain.UnitTests.AcceptanceTests;

public class ActivityAcceptanceTests
{
    private IActivityRepository _repositoryMock;
    private ActivityFactory _factory;
    private Guid _userId;
    private ActivityType _wakeUpAT;
    private ActivityType _weightAT;
    private ActivityType _sleepDurationAT;
    private DateTime _activityDate;

    [SetUp]
    public void SetUp()
    {
        // Создаем свежий мок репозитория перед каждым тестом
        _repositoryMock = Substitute.For<IActivityRepository>();

        // Передаем мок в конструктор фабрики
        _factory = new ActivityFactory(_repositoryMock);

        _userId = new Guid();

        _wakeUpAT = new ActivityType(SystemActivityTypes.WakeUp,
            "Время пробуждения",
            MetricType.TimeOfDay,
            1);

        _weightAT = new ActivityType(SystemActivityTypes.BodyWeight,
            "Вес тела",
            MetricType.Weight,
            2);

        _sleepDurationAT = new ActivityType(_userId,
            "Длительность сна",
            MetricType.Duration,
            2);

        _activityDate = DateTime.Today;
    }

    [Test]
    [TestCase("0", 0)]
    [TestCase("75.5", 75.5)]
    [TestCase("0.5", 0.5)]
    public async Task CreateActivity_WeightWithValidData_Success(string weightStr, decimal weightDec)
    {
        // Given (Дано: Вес в виде строки)
        // When (Когда: пользователь сохраняет Вес)
        var activity = await _factory.CreateActivity(_userId, _weightAT, _activityDate, weightStr) as WeightActivity;
        // Then (Тогда: Событие с указанием Веса создано корректно, инварианты соблюдены)
        activity.ShouldNotBeNull();
        activity.Weight.ShouldBe(new Weight(weightDec));
        activity.UserId.ShouldBe(_userId);
        activity.ActivityType.ShouldBe(_weightAT);
    }

    [Test]
    [TestCase("-1")]
    public async Task CreateActivity_WeightWithInvalidData_UnsupportedWeightException(string weightStr)
    {
        // Given (Дано: Неверный вес в виде строки)
        // When (Когда: пользователь сохраняет Вес)
        // Then (Тогда: Событие с указанием не создаётся)
        Should.Throw<UnsupportedWeightException>(() => _factory.CreateActivity(_userId, _weightAT, _activityDate, weightStr, CancellationToken.None));
    }

    [TestCase("НеверныйФорматВеса")]
    [TestCase(null)]
    public async Task CreateActivity_WeightWithInvalidParsingData_DomainException(string? weightStr)
    {
        // Given (Дано: Вес в виде строки, которые содержит недопустимые символы)
        // When (Когда: пользователь сохраняет Вес)
        // Then (Тогда: Событие с указанием не создаётся)
        Should.Throw<DomainException>(() => _factory.CreateActivity(_userId, _weightAT, _activityDate, weightStr, CancellationToken.None));
    }

    [Test]
    [TestCase("1", "00:01:00")] //1 минута
    [TestCase("180", "03:00:00")] //3 часа = 180 минут
    [TestCase("1440", "1.00:00:00")] //1 сутки = 24 часа = 2400 минут
    [TestCase("00:01:00", "00:01:00")]//1 минута
    [TestCase("03:00:00", "03:00:00")]//3 часа = 180 минут
    [TestCase("23:00:00", "23:00:00")]//23 часа, 24 часа и больше в таком формате невозможно
    public async Task CreateActivity_DurationWithValidData_Success(string inDur, string outDur)
    {
        // Given (Дано: Длительность в виде строки)
        // When (Когда: пользователь сохраняет Событие с длительностью)
        var activity = await _factory.CreateActivity(_userId, _sleepDurationAT, _activityDate, inDur) as DurationActivity;
        // Then (Тогда: Событие с указанием Длительности создано корректно, инварианты соблюдены)
        activity.ShouldNotBeNull();
        activity.Duration.Value.ShouldBe(TimeSpan.Parse(outDur));
        activity.UserId.ShouldBe(_userId);
        activity.ActivityType.ShouldBe(_sleepDurationAT);
    }

    [Test]
    [TestCase("0")] //0 минут
    [TestCase("-1")] //-1 минута
    public async Task CreateActivity_DurationWithInvalidData_UnsupportedWeightException(string inDur)
    {
        // Given (Дано: Неверная Длительность в виде строки)
        // When (Когда: пользователь сохраняет Событие с длительностью)
        // Then (Тогда: Событие не создаётся)
        Should.Throw<UnsupportedDurationException>(() => _factory.CreateActivity(_userId, _sleepDurationAT, _activityDate, inDur, CancellationToken.None));
    }

    [TestCase("НеверныйФорматДлительности")]
    [TestCase("1:1:1")]
    [TestCase(null)]
    public async Task CreateActivity_DurationWithInvalidParsingData_DomainException(string? inDur)
    {
        // Given (Дано: Длительность в виде строки, которые содержит недопустимые символы)
        // When (Когда: пользователь сохраняет Событие с длительностью)
        // Then (Тогда: Событие с указанием не создаётся)
        Should.Throw<DomainException>(() => _factory.CreateActivity(_userId, _sleepDurationAT, _activityDate, inDur, CancellationToken.None));
    }

    [Test]
    [TestCase("23:20", "23:20")]
    [TestCase("26.10.2026 23:00", "26.10.2026 23:00")]
    public async Task CreateActivity_TimeOfDayWithValidData_Success(string inDt, string outDt)
    {
        // Given (Дано: Время текущего дня в виде строки или Дата и время)
        // When (Когда: Пользователь сохраняет Событие)
        var activity = await _factory.CreateActivity(_userId, _wakeUpAT, _activityDate, inDt) as TimeOfDayActivity;
        // Then (Тогда: Событие с указанием времени создано корректно, инварианты соблюдены)
        activity.ShouldNotBeNull();

        DateTime? outDateTime = null;
        CultureInfo ruCI = new CultureInfo("ru-RU");
        if (outDt.Length > 5)
        {
            outDateTime = DateTime.Parse(outDt, ruCI);
        }
        else
        {
            outDateTime = DateTime.ParseExact(outDt, "H:mm", ruCI, DateTimeStyles.None);
        }

        activity.ActivityDate.ShouldBe(outDateTime.Value, TimeSpan.Zero);
        activity.UserId.ShouldBe(_userId);
        activity.ActivityType.ShouldBe(_wakeUpAT);
    }

    [Test]
    [TestCase(null)] //в таком случае дата задаётся в другом поле со строгим типом
    public async Task CreateActivity_TimeOfDayWithNullMetric_Success(string? inDt)
    {
        // Given (Дано: Дата и время события не указана в виде строки)
        // When (Когда: Пользователь сохраняет Событие)
        var activity = await _factory.CreateActivity(_userId, _wakeUpAT, _activityDate, inDt) as TimeOfDayActivity;
        // Then (Тогда: Событие создано, дата и время указаны не в виде строки, инварианты соблюдены)
        activity.ShouldNotBeNull();
        activity.ActivityDate.ShouldBe(_activityDate);
        activity.UserId.ShouldBe(_userId);
        activity.ActivityType.ShouldBe(_wakeUpAT);
    }

    [Test]
    [TestCase("НеверныйФорматДаты")]
    [TestCase("26.13.2026 23:00")]    
    public async Task CreateActivity_TimeOfDayWithInvalidParsingData_DomainException(string? inDt)
    {
        // Given (Дано: Длительность в виде строки, которые содержит недопустимые символы)
        // When (Когда: пользователь сохраняет Событие с длительностью)
        // Then (Тогда: Событие с указанием не создаётся)
        Should.Throw<DomainException>(() => _factory.CreateActivity(_userId, _wakeUpAT, _activityDate, inDt, CancellationToken.None));
    }

    [Test]
    public async Task CreateActivity_WithDayLimit_Success()
    {
        _repositoryMock.IsActivityLimitReachedAsync(
            Arg.Any<Guid>(),
            Arg.Any<ActivityType>(),
            Arg.Any<DateTime>(),
            Arg.Any<CancellationToken>())
        .Returns(false, true);

        // Given (Дано: Тип события - Вес тела, разрешено 2 взвешивания в день, т.е. 2 события)
        // When (Когда: пользователь создаёт два события
        // Then (Тогда: Первое создаётся, второе падает на лимите)
        var activity = await _factory.CreateActivity(_userId, _wakeUpAT, _activityDate, null) as TimeOfDayActivity;

        Should.Throw<DomainException>(() => _factory.CreateActivity(_userId, _wakeUpAT, _activityDate, null, CancellationToken.None));
    }

}
