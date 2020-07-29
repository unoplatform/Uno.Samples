using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Uno.Extensions;

namespace UnoChat.Client
{
    public class ViewModel : INotifyPropertyChanged
    {
        private readonly Guid _id = Guid.NewGuid();
        private readonly Guid _deviceTypeId = Guid.Empty;

        private readonly MVx.Observable.Property<string> _name;
        private readonly MVx.Observable.Property<HubConnectionState> _state;
        private readonly MVx.Observable.Command _connect;

        private readonly MVx.Observable.Property<Message.Model> _lastMessageReceived;
        private readonly MVx.Observable.Property<IEnumerable<Message.Model>> _allMessages;
        private readonly MVx.Observable.Property<string> _messageToSend;
        private readonly MVx.Observable.Property<bool> _messageToSendIsEnabled;
        private readonly MVx.Observable.Command _sendMessage;

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
            _name = new MVx.Observable.Property<string>(DefaultName, nameof(Name), args => PropertyChanged?.Invoke(this, args));
            _state = new MVx.Observable.Property<HubConnectionState>(HubConnectionState.Disconnected, nameof(State), args => PropertyChanged?.Invoke(this, args));
            _connect = new MVx.Observable.Command();
            _lastMessageReceived = new MVx.Observable.Property<Message.Model>(nameof(LastMessageReceived), args => PropertyChanged?.Invoke(this, args));
            _allMessages = new MVx.Observable.Property<IEnumerable<Message.Model>>(Enumerable.Empty<Message.Model>(), nameof(AllMessages), args => PropertyChanged?.Invoke(this, args));
            _messageToSend = new MVx.Observable.Property<string>(nameof(MessageToSend), args => PropertyChanged?.Invoke(this, args));
            _messageToSendIsEnabled = new MVx.Observable.Property<bool>(false, nameof(MessageToSendIsEnabled), args => PropertyChanged?.Invoke(this, args));
            _sendMessage = new MVx.Observable.Command();

            _connection = new HubConnectionBuilder()
                .WithUrl("https://unochatservice20200716114254.azurewebsites.net/ChatHub")
                //.WithUrl("http://localhost:61877")
                .WithAutomaticReconnect()
                .Build();
        }

        private IDisposable ShouldEnableConnectWhenNotConnected()
        {
            return _state
                .Select(state => state == HubConnectionState.Disconnected)
                .ObserveOn(Schedulers.Dispatcher)
                .Subscribe(_connect);
        }

        private IDisposable ShouldEnableMessageToSendWhenConnected()
        {
            return _state
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
                    }))
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
                .WithLatestFrom(_allMessages, (message, messages) => messages.Concat(message).ToArray())
                .Subscribe(_allMessages);
        }

        private IDisposable ShouldEnableSendMessageWhenConnectedAndBothNameAndMessageToSendAreNotEmpty()
        {
            return Observable
                .CombineLatest(_state, _name, _messageToSend, (state, name, message) => state == HubConnectionState.Connected && !(string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(message)))
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

        public IDisposable Activate(IObservable<object> messageToSendBoxReturn)
        {
            return new CompositeDisposable(
                ShouldEnableConnectWhenNotConnected(),
                ShouldEnableMessageToSendWhenConnected(),
                ShouldConnectToServiceWhenConnectInvoked(),
                ShouldDisconnectFromServiceWhenDisposed(),
                ShouldListenForNewMessagesFromTheService(),
                ShouldAddNewMessagesToAllMessages(),
                ShouldEnableSendMessageWhenConnectedAndBothNameAndMessageToSendAreNotEmpty(),
                ShouldSendMessageToServiceThenClearSentMessage(messageToSendBoxReturn)
            );
        }

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        public HubConnectionState State => _state.Get();

        public Message.Model LastMessageReceived => _lastMessageReceived.Get();

        public IEnumerable<Message.Model> AllMessages => _allMessages.Get();

        public string MessageToSend
        {
            get => _messageToSend.Get();
            set => _messageToSend.Set(value);
        }

        public bool MessageToSendIsEnabled => _messageToSendIsEnabled.Get();

        public ICommand Connect => _connect;

        public ICommand SendMessage => _sendMessage;
    }
}
