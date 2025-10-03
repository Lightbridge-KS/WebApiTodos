namespace WebApiTodos.Models
{
    public record Todo(
    int Id,
    string Name,
    DateTime DueDate,
    bool IsCompleted
);
}
