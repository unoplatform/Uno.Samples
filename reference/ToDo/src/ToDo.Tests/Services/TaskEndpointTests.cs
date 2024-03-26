using NUnit.Framework;
using System.Threading;
using ToDo.Data;
using ToDo.Data.Models;

namespace ToDo.Tests.Services;

internal class TaskEndpointTests : BaseEndpointTests<ITaskEndpoint>
{

	[SetUp]
	public void Setup() { }

	[Test]
	public async System.Threading.Tasks.Task Create_TodoTask_ShouldReturn_NewTask()
	{
		//Arrange
		var listId = "AQMkADAwATNiZmYAZC00YjBmLWQzOTItMDACLTAwCgAuAAADIptfVB-VcUaFb7L0jgOsSQEAcYiQalobw0a8Voz8RAJUmAAAAjxiAAAA";
		var newTask = new TaskData { Title = "new task created from unit test" };

		//Act
		var result = await service.CreateAsync(listId, newTask, CancellationToken.None);

		//Assert
		Assert.IsInstanceOf<TaskData>(result);
	}

	[Test]
	public async System.Threading.Tasks.Task Get_TodoTask_ShouldReturnTask()
	{
		//Arrange
		var listId = "AQMkADAwATNiZmYAZC00YjBmLWQzOTItMDACLTAwCgAuAAADIptfVB-VcUaFb7L0jgOsSQEAcYiQalobw0a8Voz8RAJUmAAAAjxiAAAA";
		var taskId = "AQMkADAwATNiZmYAZC00YjBmLWQzOTItMDACLTAwCgBGAAADIptfVB-VcUaFb7L0jgOsSQcAcYiQalobw0a8Voz8RAJUmAAAAjxiAAAAcYiQalobw0a8Voz8RAJUmAAAAo0oAAAA";

		//Act
		var result = await service.GetAsync(listId, taskId, CancellationToken.None);

		//Assert
		Assert.IsInstanceOf<TaskData>(result);
	}
}
