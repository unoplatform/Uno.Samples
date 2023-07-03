using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ChatUI.Entities;

namespace ChatUI;

public class MainPageViewModel : INotifyPropertyChanged
{
	private ObservableCollection<Message> _messages;
	private string _currentMessage;
	private bool _isMessageSent;

	public event PropertyChangedEventHandler PropertyChanged;

	public MainPageViewModel()
	{
		_messages = new ObservableCollection<Message>(Enumerable
			.Range(0, 100)
			.Select(i => new Message($"Message {i}", i % 2 == 0 ? "Me" : "Sender", DateTimeOffset.Now - TimeSpan.FromMinutes(30 * i), i % 2 == 0)));

		AddMessage = new AddMessageCommand(ExecuteAddMessage);
	}

	public ObservableCollection<Message> Messages
	{
		get => _messages;
		set
		{
			_messages = value;
			OnPropertyChanged();
		}
	}

	public string CurrentMessage
	{
		get => _currentMessage;
		set
		{
			_currentMessage = value;
			OnPropertyChanged();
		}
	}

	public bool IsMessageSent
	{
		get => _isMessageSent;
		set
		{
			_isMessageSent = value;
			OnPropertyChanged();
		}
	}

	public ICommand AddMessage { get; }

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	private void ExecuteAddMessage(string newMessageContent)
	{
		if(!string.IsNullOrEmpty(newMessageContent))
		{
			IsMessageSent = false;

			var newMessage = new Message(newMessageContent, "Me", DateTime.Now, true);

			// If the last message was from the same sender, set its IsLastInSequence property to false
			if (Messages.Count > 0 && Messages.First().IsMyMessage == newMessage.IsMyMessage)
			{
				Messages.First().IsLastInSequence = false;
			}

			// The new message is the last in the sequence, so set its IsLastInSequence property to true
			newMessage.IsLastInSequence = true;

			// Add the new message
			Messages.Insert(0, newMessage);

			// Reset the current message
			CurrentMessage = string.Empty;

			IsMessageSent = true;
		}
	}

	public class AddMessageCommand : ICommand
	{
		private Action<string> _execute;

		public AddMessageCommand(Action<string> execute)
		{
			_execute = execute;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			_execute((string)parameter);
		}
	}
}
