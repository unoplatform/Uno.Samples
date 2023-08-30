using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Windows.Input;
using Microsoft.AspNetCore.SignalR.Client;
using UnoChat.Client.Message;


namespace UnoChat.Client
{

    public enum State
    {
        SignIn,
        Connecting,
        Chatting
    }
    public class ViewModel: INotifyPropertyChanged
    {
        private readonly Guid _id = Guid.NewGuid();
        private readonly Guid _deviceTypeId = Guid.Empty;

        private readonly MVx.Observable.Property<State> _state;
        private readonly MVx.Observable.Property<string> _name;
        private readonly MVx.Observable.Property<HubConnectionState> _hubState;
        private readonly MVx.Observable.Command _connect;

        private readonly MVx.Observable.Property<Message.Model> _lastMessageReceived;
        private readonly MVx.Observable.Property<string> _messageToSend;
        private readonly MVx.Observable.Property<bool> _messageToSendIsEnabled;
        private readonly MVx.Observable.Command _sendMessage;
        private readonly MVx.Observable.Property<string> _currentTheme;
        private readonly MVx.Observable.Command _toggleTheme;

        private readonly ObservableCollection<Message.Model> _allMessages;

        private readonly HubConnection _connection;

        public event PropertyChangedEventHandler PropertyChanged;
        private static string DefaultName => typeof(ViewModel)
        .Assembly
        .GetName()
        .Name
            .Split('.')
        .Last();

        public ViewModel()
        {
            _state = new MVx.Observable.Property<State>(State.SignIn, nameof(State), args => PropertyChanged?.Invoke(this, args));
            _name = new MVx.Observable.Property<string>(DefaultName, nameof(Name), args => PropertyChanged?.Invoke(this, args));
            _hubState = new MVx.Observable.Property<HubConnectionState>(HubConnectionState.Disconnected, nameof(HubState), args => PropertyChanged?.Invoke(this, args));
            _connect = new MVx.Observable.Command();
            _lastMessageReceived = new MVx.Observable.Property<Message.Model>(nameof(LastMessageReceived), args => PropertyChanged?.Invoke(this, args));
            _messageToSend = new MVx.Observable.Property<string>(nameof(MessageToSend), args => PropertyChanged?.Invoke(this, args));
            _messageToSendIsEnabled = new MVx.Observable.Property<bool>(false, nameof(MessageToSendIsEnabled), args => PropertyChanged?.Invoke(this, args));
            _currentTheme = new MVx.Observable.Property<string>("Light", nameof(CurrentTheme), args => PropertyChanged?.Invoke(this, args));
            _toggleTheme = new MVx.Observable.Command(true);
            _sendMessage = new MVx.Observable.Command();


            _allMessages = new ObservableCollection<Message.Model>();

            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7167/ChatHub")
                //.WithUrl("http://localhost:61877")
                .WithAutomaticReconnect()
                .Build();
        }

        private IDisposable ShouldEnableConnectWhenNotConnected()
        {
            return _hubState
                .Select(state => state == HubConnectionState.Disconnected)
                .ObserveOn(Schedulers.Dispatcher)
                .Subscribe(_connect);
        }

        private IDisposable ShouldEnableMessageToSendWhenConnected()
        {
            return _hubState
                .Select(state => state == HubConnectionState.Connected)
                .Subscribe(_messageToSendIsEnabled);
        }

        private IDisposable ShouldConnectToServiceWhenConnectInvoked()
        {
            return _connect
                .SelectMany(_ => Observable
                    .StartAsync(async () =>
                    {
                        await _connection.StartAsync();
                        return _connection.State;
                    })
                    // Ensure the transition takes at least three seconds so the user sees the connecting animation
                    .Zip(Observable.Interval(TimeSpan.FromSeconds(3), Schedulers.Default), (state, _) => state)
                    .StartWith(HubConnectionState.Connecting))
                .ObserveOn(Schedulers.Dispatcher)
                .Subscribe(_hubState);
        }

        private static State? MapToState(HubConnectionState state)
        {
            return state switch
            {
                HubConnectionState.Disconnected => State.SignIn,
                HubConnectionState.Connecting => State.Connecting,
                HubConnectionState.Connected => State.Chatting,
                HubConnectionState.Reconnecting => State.Connecting,
                _ => null
            };
        }

        private IDisposable ShouldUpdateStateWhenHubStateChanges()
        {
            return _hubState
                .Select(MapToState)
                .Where(newState => newState.HasValue)
                .Select(newState => newState.Value)
                .ObserveOn(Schedulers.Dispatcher)
                .Subscribe(_state);
        }

        private IDisposable ShouldDisconnectFromServiceWhenDisposed()
        {
            return Disposable.Create(() => _ = _connection.StopAsync());
        }

        private IDisposable ShouldListenForNewMessagesFromTheService()
        {
            return Observable
                .Create<Message.Model>(
                    observer =>
                    {
                        Action<DateTimeOffset, DateTimeOffset, Guid, string, Guid, string> onReceiveMessage =
                            (sentAt, relayedAt, userId, userName, deviceTypeId, message) => observer.OnNext(
                                new Message.Model
                                {
                                    Sender = new Message.Sender
                                    {
                                        Id = userId,
                                        Name = userName,
                                        DeviceTypeId = deviceTypeId,
                                        IsMe = userId == _id
                                    },
                                    Text = message,
                                    SentAt = sentAt,
                                    RelayedAt = relayedAt,
                                    ReceivedAt = DateTimeOffset.UtcNow
                                }
                            );

                        return _connection.On("ReceiveMessage", onReceiveMessage);
                    })
                .ObserveOn(Schedulers.Dispatcher)
                .Subscribe(_lastMessageReceived);
        }

        private IDisposable ShouldAddNewMessagesToAllMessages()
        {
            return _lastMessageReceived
                .Where(message => message != null)
                .Do(message => _allMessages.Add(message))
                .Subscribe();
        }

        private IDisposable ShouldEnableSendMessageWhenConnectedAndBothNameAndMessageToSendAreNotEmpty()
        {
            return Observable
                .CombineLatest(_hubState, _name, _messageToSend, (state, name, message) => state == HubConnectionState.Connected && !(string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(message)))
                .Subscribe(_sendMessage);
        }

        private IDisposable ShouldSendMessageToServiceThenClearSentMessage(IObservable<object> messageToSendBoxReturn)
        {
            var namedMessage = Observable
                .CombineLatest(_name, _messageToSend, (name, message) => (Id: _id, Name: name, DeviceTypeId: _deviceTypeId, Message: message));

            return Observable.Merge(_sendMessage, messageToSendBoxReturn)
                .WithLatestFrom(namedMessage, (_, tuple) => tuple)
                .Where(tuple => !string.IsNullOrEmpty(tuple.Message))
                .SelectMany(tuple => Observable
                    .StartAsync(() => _connection.InvokeAsync("SendMessage", DateTimeOffset.UtcNow, tuple.Id, tuple.Name, tuple.DeviceTypeId, tuple.Message)))
                .Select(_ => string.Empty)
                .ObserveOn(Schedulers.Dispatcher)
                .Subscribe(_messageToSend);
        }

        private IDisposable ShouldToggleThemeWhenToggleThemeInvoked()
        {
            return _toggleTheme
                .WithLatestFrom(_currentTheme, (_, theme) => theme.Equals("Dark", StringComparison.OrdinalIgnoreCase) ? "Light" : "Dark")
                .Subscribe(_currentTheme);
        }

        private IDisposable ShouldSendThemeToThemeObserver(IObserver<string> themeObserver)
        {
            return _currentTheme
                .Skip(1) // Don't emit the default value
                .Subscribe(themeObserver);
        }

        private IDisposable ShouldSendModelsAddedToAllMessagesToMessageObserver(IObserver<Model> messageObserver)
        {
            return Observable
                .FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    handler => (s, e) => handler(e),
                    handler => _allMessages.CollectionChanged += handler,
                    handler => _allMessages.CollectionChanged -= handler)
                .Where(args => args.Action == NotifyCollectionChangedAction.Add)
                .Select(args => args.NewItems.OfType<Message.Model>().FirstOrDefault())
                .Where(model => model != null)
#if !__WASM__
                .Delay(TimeSpan.FromMilliseconds(10), Schedulers.Default) // Wait for the list view to have been updated
#endif
                .ObserveOn(Schedulers.Dispatcher)
                .Subscribe(messageObserver);
        }

        public IDisposable Activate(IObservable<object> messageToSendBoxReturn, IObserver<string> themeObserver, IObserver<Message.Model> messageObserver)
        {
			return new CompositeDisposable(
                ShouldUpdateStateWhenHubStateChanges(),
                ShouldEnableConnectWhenNotConnected(),
                ShouldEnableMessageToSendWhenConnected(),
                ShouldConnectToServiceWhenConnectInvoked(),
                ShouldDisconnectFromServiceWhenDisposed(),
                ShouldListenForNewMessagesFromTheService(),
                ShouldAddNewMessagesToAllMessages(),
                ShouldEnableSendMessageWhenConnectedAndBothNameAndMessageToSendAreNotEmpty(),
                ShouldSendMessageToServiceThenClearSentMessage(messageToSendBoxReturn),
                ShouldToggleThemeWhenToggleThemeInvoked(),
                ShouldSendThemeToThemeObserver(themeObserver),
                ShouldSendModelsAddedToAllMessagesToMessageObserver(messageObserver)
            );
        }

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        public State State => _state.Get();

        public HubConnectionState HubState => _hubState.Get();

        public Message.Model LastMessageReceived => _lastMessageReceived.Get();

        public IEnumerable<Message.Model> AllMessages => _allMessages;

        public string MessageToSend
        {
            get => _messageToSend.Get();
            set => _messageToSend.Set(value);
        }

        public bool MessageToSendIsEnabled => _messageToSendIsEnabled.Get();

        public string CurrentTheme => _currentTheme.Get();

        public ICommand ToggleTheme => _toggleTheme;

        public ICommand Connect => _connect;

        public ICommand SendMessage => _sendMessage;

    }
}
