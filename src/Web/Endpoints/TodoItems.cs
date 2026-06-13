using DailyTracker.Application.TodoItems.Commands.CreateTodoItem;
using DailyTracker.Application.TodoItems.Commands.DeleteTodoItem;
using DailyTracker.Application.TodoItems.Commands.UpdateTodoItem;
using DailyTracker.Application.TodoItems.Commands.UpdateTodoItemDetail;
using Microsoft.AspNetCore.Http.HttpResults;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DailyTracker.Web.Endpoints;

public class TodoItems : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(HandleTelegramWebhook, "telegram-webhook/{token}");
    }

    [EndpointSummary("sum")]
    [EndpointDescription("descr.")]
    public static async Task<Results<Ok, UnauthorizedHttpResult>> HandleTelegramWebhook(
        string token,
        Update update,
        ISender sender,
        IConfiguration configuration)
    {
        // 1. Извлекаем реальный токен бота из конфигурации (например, из appsettings.json или User Secrets)
        var botToken = configuration["TelegramBotToken"];

        // 2. Сверяем токен из URL с нашим реальным токеном
        if (string.IsNullOrEmpty(botToken) || token != botToken)
        {
            // Если токены не совпали, возвращаем 401 Unauthorized и прерываем выполнение
            return TypedResults.Unauthorized();
        }

        // Обязательно возвращаем 200 OK, чтобы Telegram не присылал это событие повторно
        return TypedResults.Ok();
    }
}
