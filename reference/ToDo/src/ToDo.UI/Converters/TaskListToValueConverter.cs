using System;
using System.Collections.Generic;
using System.Text;
using ToDo.Business.Models;
using Uno.Extensions.Reactive.Bindings;

namespace ToDo.Converters
{
	public class TaskListToValueConverter : IValueConverter
	{
		public object? ImportantValue { get; set; }
		public object? TasksValue { get; set; }
		public object? DefaultValue { get; set; }

		public object? Convert(object value, Type targetType, object parameter, string language)
		{
			return TypeCast()?.WellknownListName switch
			{
				TaskList.WellknownListNames.Important => ImportantValue,
				TaskList.WellknownListNames.Tasks => TasksValue,

				_ => DefaultValue,
			};

			TaskList? TypeCast() =>
				value as TaskList ??
				(value as Bindable<TaskList>)?.GetValue();
		}

		public object? ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException("Only one-way conversion is supported.");
	}
}
