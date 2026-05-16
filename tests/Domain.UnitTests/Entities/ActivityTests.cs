using DailyTracker.Domain.Constants;
using DailyTracker.Domain.Entities;
using DailyTracker.Domain.Entities.Activities;
using DailyTracker.Domain.Enums;
using DailyTracker.Domain.Exceptions;
using DailyTracker.Domain.Repositories;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace DailyTracker.Domain.UnitTests.ValueObjects;

public class ActivityTests
{
    private IActivityRepository _repositoryMock;
    private ActivityFactory _factory;
    private Guid _userId;

    [SetUp]
    public void SetUp()
    {
        // Создаем свежий мок репозитория перед каждым тестом
        _repositoryMock = Substitute.For<IActivityRepository>();

        // Передаем мок в конструктор фабрики
        _factory = new ActivityFactory(_repositoryMock);

        _userId = Guid.NewGuid();
    }

    [Test]
    public async Task CreateActivity_WhenDayLimitReached_ShouldThrowDomainException()
    {
        var aDate = DateTime.Today;

        ActivityType activityType = new ActivityType(SystemActivityTypes.WakeUp,
            "Пробуждение",
            MetricType.TimeOfDay,
            1);

        _repositoryMock.IsActivityLimitReachedAsync(_userId, activityType, aDate, Arg.Any<CancellationToken>())
        .Returns(true);

        Should.Throw<DomainException>(() => _factory.CreateTimeActivityAsync(_userId, activityType, aDate, CancellationToken.None));
    }
}
