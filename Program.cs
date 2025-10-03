using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Rewrite;
using WebApiTodos.Models;
using WebApiTodos.Services;

var builder = WebApplication.CreateBuilder(args);

// DI Container
builder.Services.AddSingleton<ITaskService>(new InMemoryTaskService());

var app = builder.Build();

var todos = new List<Todo>();

// Rewrite Middleware
app.UseRewriter(new RewriteOptions().AddRedirect("tasks/(.*)", "todos/$1"));

// Request
app.MapGet("/todos", (ITaskService service) => service.GetTodos());
app.MapGet("/todos/{id}", Results<Ok<Todo>, NotFound> (int id, ITaskService service) =>
{
    var targetTodo = service.GetTodoById(id);
    return targetTodo is null
    ? TypedResults.NotFound()
    : TypedResults.Ok(targetTodo);
});

// Create
app.MapPost("/todos", (Todo task, ITaskService service) =>
{
    service.AddTodo(task);
    return TypedResults.Created("/todos/{id}", task);
})
.AddEndpointFilter(async (context, next) =>
{
    var taskArgument = context.GetArgument<Todo>(0);
    var errors = new Dictionary<string, string[]>();

    if (taskArgument.DueDate < DateTime.UtcNow)
    {
        errors.Add(nameof(Todo.DueDate), ["Cannot have due date in the past"]);
    }

    if (taskArgument.IsCompleted)
    {
        errors.Add(nameof(Todo.IsCompleted), ["Cannot add completed todo."]);
    }

    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    return await next(context);
});

// Delete
app.MapDelete("/todos/{id}", (int id, ITaskService service) =>
{
    service.DeleteTodoById(id);
    return TypedResults.NoContent();
});

app.Run();


