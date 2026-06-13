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
            new Guid(),
            "Время пробуждения",
            MetricType.TimeOfDay,
            1);

        _weightAT = new ActivityType(SystemActivityTypes.BodyWeight,
            new Guid(),
            "Вес тела",
            MetricType.Weight,
            3);

        _sleepDurationAT = new ActivityType(_userId,
            "Длительность сна",
            MetricType.Duration,
            3);

        _activityDate = DateTime.Today;
    }

    /// <summary>
    /// Пример создания обычного события (без проекта)
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task Should_Record_Weight_Activity_With_Valid_Data()
    {
        // Given (Дано: сущность типа активности "Вес")
        double inputWeight = 75.5;

        // When (Когда: пользователь фиксирует свой вес через фабрику)
        var activity = await _factory.CreateWeightActivityAsync(_userId, _weightAT, _activityDate, inputWeight);

        // Then (Тогда: объект создан корректно, инварианты соблюдены)
        activity.ShouldNotBeNull();
        activity.Weight.Value.ShouldBe(inputWeight);
        activity.UserId.ShouldBe(_userId);
        activity.ActivityType.ShouldBe(_weightAT);
    }

    /// <summary>
    /// Пример создания события которое связано с проектом
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task Should_Allow_Project_For_ActivityType()
    {
        var proj = new Project(1, "DailyTracker");

        var codingDurationAT = new ActivityType(_userId,
            "Время кодинга",
            MetricType.Duration,
            proj,
            3);

        // When (Когда: пользователь фиксирует свой вес через фабрику)
        var activity = await _factory.CreateDurationActivityAsync(_userId, codingDurationAT, _activityDate, TimeSpan.FromHours(2));

        // Then (Тогда: объект создан корректно, инварианты соблюдены)
        activity.ShouldNotBeNull();
        activity.Duration.Value.ShouldBe(TimeSpan.FromHours(2));
        activity.UserId.ShouldBe(_userId);
        activity.ActivityType.UserId.ShouldBe(_userId);
        activity.ActivityType.ShouldBe(codingDurationAT);
        activity.ActivityType.Project!.Name.ShouldBe("DailyTracker");
    }

    /// <summary>
    /// Когда достигнут дневной лимит по событиям, то получаем ошибку
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task WhenDayLimitReached_ThrowsError()
    {
        var activityDate = DateTime.Today;

        _repositoryMock.IsActivityLimitReachedAsync(_userId, _wakeUpAT, activityDate, Arg.Any<CancellationToken>())
        .Returns(true);

        //при создании второго такого события получим ошибку
        Should.Throw<DomainException>(() => _factory.CreateTimeActivityAsync(_userId, _wakeUpAT, activityDate, CancellationToken.None));
    }

    /// <summary>
    /// Вес может быть 0 и больше
    /// </summary>
    [Test]
    public void Should_Throw_Exception_When_Weight_Is_Zero_Or_Negative()
    {
        Should.Throw<UnsupportedWeightException>(() =>
             _factory.CreateWeightActivityAsync(_userId, _weightAT, _activityDate, -2))
            .Message.ShouldContain("Вес должен быть больше или равен нулю.");
    }


    /// <summary>
    /// Длительность может быть строго больше нуля
    /// </summary>
    [Test]
    public void Should_Throw_Exception_When_Duration_Is_Zero_Or_Negative()
    {
        Should.Throw<DomainException>(() =>
             _factory.CreateDurationActivityAsync(_userId, _sleepDurationAT, _activityDate, TimeSpan.FromSeconds(0)))
            .Message.ShouldContain("Длительность события не может быть меньше или ровна нуля.");

        Should.Throw<DomainException>(() =>
             _factory.CreateDurationActivityAsync(_userId, _sleepDurationAT, _activityDate, TimeSpan.FromSeconds(-10)))
            .Message.ShouldContain("Длительность события не может быть меньше или ровна нуля.");
    }
}
