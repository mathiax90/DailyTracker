using DailyTracker.Application.Activities.Commands.CreateActivityCommand;
using DailyTracker.Application.Activities.Commands.DeleteActivity;
using DailyTracker.Application.Activities.Commands.UpdateActivity;
using DailyTracker.Application.Activities.Queries.ActivityQuery;
using DailyTracker.Application.ActivityTypes.Commands.CreateActivityTypeCommand;
using DailyTracker.Application.ActivityTypes.Commands.DeleteActivityType;
using DailyTracker.Application.ActivityTypes.Commands.UpdateActivityType;
using DailyTracker.Application.ActivityTypes.Queries.ActivityTypeQuery;
using DailyTracker.Domain.Constants;
using DailyTracker.Domain.Exceptions;

namespace DailyTracker.Application.FunctionalTests.Common;

public class MainTest : CleanTestBase
{
    [Test]
    public async Task MainCase()
    {
        #region Создание События с Системным типом
        //Дано: Пользователь и Системные типы событий
        Guid userId = Guid.Parse(await TestApp.RunAsDefaultUserAsync());

        //Когда: создаём системное событие через CreateActivityCommandCommand
        DateTime activityDate = new DateTime(2026, 10, 10, 0, 0, 0, DateTimeKind.Utc);
        CreateActivityCommand createActivityCommand = new CreateActivityCommand(userId, SystemActivityTypes.Work, activityDate, "10");
        int activityId = await TestApp.SendAsync(createActivityCommand);

        //Тогда: Системное событие создано
        ActivityQuery activityQuery = new ActivityQuery(userId, activityId);
        List<ActivityDto> activities = await TestApp.SendAsync(activityQuery);

        activities.ShouldNotBeNull();
        activities.Count().ShouldBe(1);
        activities[0].Id.ShouldBe(activityId);
        activities[0].UserId.ShouldBe(userId);
        activities[0].MetricValue.ShouldBe("00:10 (10 мин.)");
        #endregion

        #region Создание Пользователького типа события
        //Дано: Пользователь

        //Когда: Создаём Пользовательский тип события
        CreateActivityTypeCommand createActivityTypeCommand = new CreateActivityTypeCommand(userId, "Длительность дневного сна", Domain.Enums.MetricType.Duration, null, 1);
        int daytimeSleepATId = await TestApp.SendAsync(createActivityTypeCommand);

        //Тогда: Тип события создан
        ActivityTypeQuery activityTypeQuery = new ActivityTypeQuery(userId, daytimeSleepATId);
        List<ActivityTypeDto> activityTypes = await TestApp.SendAsync(activityTypeQuery);

        activityTypes.ShouldNotBeNull();
        activityTypes.Count().ShouldBe(1);
        activityTypes[0].Id.ShouldBe(daytimeSleepATId);
        activityTypes[0].UserId.ShouldBe(userId);
        activityTypes[0].Name.ShouldBe("Длительность дневного сна");
        #endregion

        #region Изменение Имени Пользователького типа события
        //Дано: пользовательский тип события

        //Когда: Создаём Пользовательский тип события
        UpdateActivityTypeCommand updateActivityTypeCmd = new UpdateActivityTypeCommand(userId, daytimeSleepATId, "Дневной сон");
        await TestApp.SendAsync(updateActivityTypeCmd);

        //Тогда: Тип события создан
        activityTypeQuery = new ActivityTypeQuery(userId, daytimeSleepATId);
        activityTypes = await TestApp.SendAsync(activityTypeQuery);

        activityTypes.ShouldNotBeNull();
        activityTypes.Count().ShouldBe(1);
        activityTypes[0].Id.ShouldBe(daytimeSleepATId);
        activityTypes[0].UserId.ShouldBe(userId);
        activityTypes[0].Name.ShouldBe("Дневной сон");
        #endregion

        #region Получение всех типов событий для пользователя
        //Дано: Пользователь с одним Пользовательским типом события и тремя системными

        //Когда: Запрашиваем типы событий
        activityTypeQuery = new ActivityTypeQuery(userId, null);
        activityTypes = await TestApp.SendAsync(activityTypeQuery);

        //Тогда: Получаем 4 типа событий
        activityTypes.ShouldNotBeNull();
        activityTypes.Count().ShouldBe(4);
        #endregion

        #region Создание События с Пользовательским типом события
        //Дано: Пользователь и Пользовательский тип события

        //Когда: Создаём Событие через CreateActivityCommandCommand
        activityDate = new DateTime(2026, 10, 10, 0, 0, 0, DateTimeKind.Utc);
        
        createActivityCommand = new CreateActivityCommand(userId, daytimeSleepATId, activityDate, "20");
        int daytimeSleepActivityId = await TestApp.SendAsync(createActivityCommand);

        //Тогда: Событие создано
        activityQuery = new ActivityQuery(userId, daytimeSleepActivityId);
        activities = await TestApp.SendAsync(activityQuery);

        activities.ShouldNotBeNull();
        activities.Count().ShouldBe(1);
        activities[0].Id.ShouldBe(daytimeSleepActivityId);
        activities[0].UserId.ShouldBe(userId);
        activities[0].MetricValue.ShouldBe("00:20 (20 мин.)");
        #endregion

        #region Изменение События с Пользовательским типом события
        //Дано: Событие с Пользовательским типом события
        ActivityDto userTypeActivityDto = activities[0];

        //Когда: Изменяем метрику события
        UpdateActivityCommand updateActivityCommand = new UpdateActivityCommand(userTypeActivityDto.UserId, userTypeActivityDto.Id, userTypeActivityDto.ActivityDate, "30");
        await TestApp.SendAsync(updateActivityCommand);

        //Тогда: Метрика меняется
        activityQuery = new ActivityQuery(userId, daytimeSleepActivityId);
        activities = await TestApp.SendAsync(activityQuery);

        activities.ShouldNotBeNull();
        activities.Count().ShouldBe(1);
        activities[0].Id.ShouldBe(daytimeSleepActivityId);
        activities[0].UserId.ShouldBe(userId);
        activities[0].MetricValue.ShouldBe("00:30 (30 мин.)");
        #endregion

        #region Добавление события с превышением дневного лимита
        createActivityCommand = new CreateActivityCommand(userId, daytimeSleepATId, activityDate, "20");

        var limitException = await Should.ThrowAsync<DomainException>(() => TestApp.SendAsync(createActivityCommand));
        limitException.Message.ShouldContain("Достигнут лимит событий");
        #endregion

        #region Удаление Пользовательского типа события с получением ошибки
        DeleteActivityTypeCommand deleteActivityTypeCmd = new DeleteActivityTypeCommand(userId, daytimeSleepATId);
        
        var hasChildException = await Should.ThrowAsync<DomainException>(() => TestApp.SendAsync(deleteActivityTypeCmd));
        hasChildException.Message.ShouldBe("Невозможно удалить объект, так как на него ссылаются другие записи.");
        #endregion

        #region Удаление События с пользовательским типом
        DeleteActivityCommand deleteActivityCmd = new DeleteActivityCommand(userId, userTypeActivityDto.Id);
        await TestApp.SendAsync(deleteActivityCmd);
        
        DateTime fromDate = new DateTime(2026, 10, 10, 0, 0, 0, DateTimeKind.Utc);
        DateTime toDate = new DateTime(2026, 10, 11, 0, 0, 0, DateTimeKind.Utc);

        activityQuery = new ActivityQuery(userId, null, fromDate, toDate);
        activities = await TestApp.SendAsync(activityQuery);

        activities.ShouldNotBeNull();
        activities.Count().ShouldBe(1);
        #endregion

        #region Удаление Пользовательского типа события с успехом
        //Дано: Пользователь с 4 Типами событий


        //Когда: Удаляем типы событий и запрашиваем типы событий пользователя
        await TestApp.SendAsync(deleteActivityTypeCmd);

        activityTypeQuery = new ActivityTypeQuery(userId, null);
        activityTypes = await TestApp.SendAsync(activityTypeQuery);

        //Тогда: Получаем 3 типа событий
        activityTypes.ShouldNotBeNull();
        activityTypes.Count().ShouldBe(3);

        #endregion
    }
}
