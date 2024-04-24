namespace ToDo.Business.Models;

public static class ToDoTaskExtensions
{
	public static ToDoTask ToggleIsCompleted(this ToDoTask task)
		=> task with { Status = task.IsCompleted ? ToDoTask.TaskStatus.NotStarted : ToDoTask.TaskStatus.Completed };

	public static ToDoTask ToggleIsImportant(this ToDoTask task)
		=> task with { Importance = task.IsImportant ? ToDoTask.TaskImportance.Normal : ToDoTask.TaskImportance.Important };
}
