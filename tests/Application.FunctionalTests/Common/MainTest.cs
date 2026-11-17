using DailyTracker.Application.Activities.Commands.CreateActivityCommand;
using DailyTracker.Application.Activities.Queries.ActivityQuery;
using DailyTracker.Application.ActivityTypes.Commands.CreateActivityTypeCommand;
using DailyTracker.Application.ActivityTypes.Queries.ActivityTypeQuery;
using DailyTracker.Domain.Constants;

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
        CreateActivityTypeCommand createActivityTypeCommand = new CreateActivityTypeCommand(userId, "Длительность дневного сна", Domain.Enums.MetricType.Duration, null, 2);
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

        #region Получение всех типов событий для пользователя
        //Дано: Пользователь с одним Пользовательским типом события и тремя системными

        //Дано: Пользователь с одним Типом события и тремя системными

        //Когда: Запрашиваем типы событий
        activityTypeQuery = new ActivityTypeQuery(userId, null);
        activityTypes = await TestApp.SendAsync(activityTypeQuery);

        //Тогда: Получаем 4 типа событий
        activityTypes.ShouldNotBeNull();
        activityTypes.Count().ShouldBe(4);
        #endregion


        //item.ShouldNotBeNull();
        //    item!.ListId.ShouldBe(command.ListId);
        //    item.Title.ShouldBe(command.Title);
        //    item.CreatedBy.ShouldBe(userId);
        //    item.Created.ShouldBe(DateTime.Now, TimeSpan.FromMilliseconds(10000));
        //    item.LastModifiedBy.ShouldBe(userId);
        //    item.LastModified.ShouldBe(DateTime.Now, TimeSpan.FromMilliseconds(10000));

        //CreateActivityTypeCommandCommand atCommand = new CreateActivityTypeCommandCommand(userId, "", activityDate, "10");
    }

    //[Test]
    //public async Task ShouldCreateTodoItem()
    //{
    //    var userId = await TestApp.RunAsDefaultUserAsync();

    //    var listId = await TestApp.SendAsync(new CreateTodoListCommand
    //    {
    //        Title = "New List"
    //    });

    //    var command = new CreateTodoItemCommand
    //    {
    //        ListId = listId,
    //        Title = "Tasks"
    //    };

    //    var itemId = await TestApp.SendAsync(command);

    //    var item = await TestApp.FindAsync<TodoItem>(itemId);

    //    item.ShouldNotBeNull();
    //    item!.ListId.ShouldBe(command.ListId);
    //    item.Title.ShouldBe(command.Title);
    //    item.CreatedBy.ShouldBe(userId);
    //    item.Created.ShouldBe(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    //    item.LastModifiedBy.ShouldBe(userId);
    //    item.LastModified.ShouldBe(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    //}
}
