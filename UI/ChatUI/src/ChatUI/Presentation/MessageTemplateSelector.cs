using System;
using ChatUI.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ChatUI.Presentation;

public class MessageTemplateSelector : DataTemplateSelector
{
	public DataTemplate MyMessageTemplate { get; set; }
	public DataTemplate OtherMessageTemplate { get; set; }

	protected override DataTemplate SelectTemplateCore(object item)
	{
		if (item != null)
		{
			return item switch
			{
				Message { IsMyMessage: true } => MyMessageTemplate,
				Message { IsMyMessage: false } => OtherMessageTemplate,
				_ => throw new InvalidOperationException(),
			};
		}

		return MyMessageTemplate;
	}
}