using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChatUI.Entities;

public record Message(string Text, string ContactName, DateTimeOffset Timestamp, bool IsMyMessage) : INotifyPropertyChanged
{
	private bool _isLastInSequence = true;

	public event PropertyChangedEventHandler PropertyChanged;

	public string UserFriendlyTimestamp => Timestamp.LocalDateTime.ToString("t");

	public bool IsLastInSequence
	{
		get =>  _isLastInSequence;
		set 
		{ 
			_isLastInSequence = value;
			OnPropertyChanged();
		}
	}

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
