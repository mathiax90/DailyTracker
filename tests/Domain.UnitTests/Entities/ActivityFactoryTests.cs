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

namespace DailyTracker.Domain.UnitTests.ValueObjects;

public class ActivityFactoryTests
{
    private IActivityRepository _repositoryMock;
    private ActivityFactory _factory;
    private Guid _userId;
    private ActivityType _wakeUpAT;
    private ActivityType _weightAT;
    private ActivityType _durationAT;
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
            3);

        _durationAT = new ActivityType(_userId,
            "Зарядка",
            MetricType.Duration,
            1);

        _activityDate = DateTime.Today;
    }

    [Test]
    public async Task CreateTimeActivityAsync_ShouldReturnTimeOfDayActivity()
    {
        _repositoryMock.IsActivityLimitReachedAsync(_userId, _wakeUpAT, _activityDate, Arg.Any<CancellationToken>())
        .Returns(false);

        var a = await _factory.CreateTimeActivityAsync(_userId, _wakeUpAT, _activityDate, CancellationToken.None);
        a.ShouldBeOfType<TimeOfDayActivity>();
    }

    [Test]
    public async Task CreateWeightActivityAsync_ShouldReturnWeightActivityWithValue()
    {
        _repositoryMock.IsActivityLimitReachedAsync(_userId, _weightAT, _activityDate, Arg.Any<CancellationToken>())
        .Returns(false);

        var a = await _factory.CreateWeightActivityAsync(_userId, _weightAT, _activityDate, 10, CancellationToken.None);
        a.ShouldBeOfType<WeightActivity>();

        a.Weight.Value.ShouldBe(10);
    }

    [Test]
    public async Task CreateDurationActivityAsync_ShouldReturnDurationActivityWithValue()
    {
        _repositoryMock.IsActivityLimitReachedAsync(_userId, _durationAT, _activityDate, Arg.Any<CancellationToken>())
            .Returns(false);

        var a = await _factory.CreateDurationActivityAsync(_userId, _durationAT, _activityDate, TimeSpan.FromSeconds(10), CancellationToken.None);
        a.ShouldBeOfType<DurationActivity>();

        a.Duration.Value.ShouldBeEquivalentTo(TimeSpan.FromSeconds(10));
    }

    [Test]
    public async Task CreateDurationActivityAsync_WhenWrongActivityType_ShouldReturnDurationActivityWithValue()
    {
        Should.Throw<DomainException>(() => _factory.CreateDurationActivityAsync(_userId, _wakeUpAT, _activityDate, TimeSpan.FromSeconds(10), CancellationToken.None));
    }
}
