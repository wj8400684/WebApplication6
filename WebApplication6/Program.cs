using System.Text.Json.Serialization;
using ServiceSelf;

if (!Service.UseServiceSelf(args))
    return;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Host.UseServiceSelf();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

var sampleTodos = new Todo[]
{
    new(1, "Walk the dog"),
    new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
    new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
    new(4, "Clean the bathroom"),
    new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
};

var todosApi = app.MapGroup("/todos");
todosApi.MapGet("/", () => sampleTodos);
todosApi.MapGet("/{id}", (int id) =>
    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
        ? Results.Ok(todo)
        : Results.NotFound());

app.Run();

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}

//
// const string serviceName = "socketServer";
// var serviceOptions = new ServiceOptions
// {
//     Arguments = new[] { new Argument("key", "value") },
//     Description = "-这是演示示例应用-",
// };
// serviceOptions.Linux.Service.Restart = "always";
// serviceOptions.Linux.Service.RestartSec = "10";
// serviceOptions.Windows.DisplayName = "-演示示例-";
// serviceOptions.Windows.FailureActionType = WindowsServiceActionType.Restart;
//
// var workingDirectory = Environment.CurrentDirectory;
// var execStart = string.Join("/", workingDirectory, "WebApplicationTarget");
//
// Console.WriteLine($"execStart: {execStart}");
//         
// serviceOptions.Linux.Service["WorkingDirectory"] = workingDirectory;
// // serviceOptions.Linux.Service["ExecStart"] = execStart;
// // serviceOptions.Linux.Service["Environment"] = "ASPNETCORE_ENVIRONMENT=Production";
// // serviceOptions.Linux.Service["User"] = "root";
// // serviceOptions.Linux.Service["LimitMEMORY"] = "5120M";
// // serviceOptions.Linux.Service["LimitNOFILE"] = "50000";
// // serviceOptions.Linux.Service["LimitCPU"] = "100%";
// serviceOptions.Linux.Service.Restart = "always";
// serviceOptions.Linux.Service.RestartSec = "10";
