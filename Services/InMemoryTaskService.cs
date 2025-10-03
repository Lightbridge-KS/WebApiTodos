using WebApiTodos.Models;

namespace WebApiTodos.Services
{
    class InMemoryTaskService : ITaskService
    {
        private readonly List<Todo> _todos = [];

        public Todo AddTodo(Todo task)
        {
            _todos.Add(task);
            return task;
        }

        public void DeleteTodoById(int id)
        {
            _todos.RemoveAll(task => id == task.Id);
        }
        public List<Todo> GetTodos()
        {
            return _todos;
        }

        public Todo? GetTodoById(int id)
        {
            return _todos.SingleOrDefault(t => id == t.Id);
        }
    }
}


