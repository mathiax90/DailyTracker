using DailyTracker.Domain.Constants;
using DailyTracker.Domain.Entities;
using DailyTracker.Domain.Entities.Activities;
using DailyTracker.Domain.Enums;
using DailyTracker.Domain.Exceptions;
using DailyTracker.Domain.Repositories;
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

    [SetUp]
    public void SetUp()
    {
        // Создаем свежий мок репозитория перед каждым тестом
        _repositoryMock = Substitute.For<IActivityRepository>();

        // Передаем мок в конструктор фабрики
        _factory = new ActivityFactory(_repositoryMock);

        _userId = Guid.NewGuid();

        _wakeUpAT = new ActivityType(SystemActivityTypes.WakeUp,
            "Пробуждение",
            MetricType.TimeOfDay,
            1);
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
}
