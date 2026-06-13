using DailyTracker.Shared;

namespace DailyTracker.TestAppHost;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        var postgresServer = builder.AddPostgres($"DailyTrackerPgsql")
            .WithImageTag("14")
            .WithEndpoint("tcp", endpoint => { endpoint.TargetPort = 5432; endpoint.Port = 5433; });
        var db = postgresServer.AddDatabase(Services.Database);

        builder.Build().Run();
    }
}
