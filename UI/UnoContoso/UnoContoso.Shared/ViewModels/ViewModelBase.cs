using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using UnoContoso.Models.Consts;

namespace UnoContoso.ViewModels
{
    public abstract class ViewModelBase : BindableBase, IActiveAware, INavigationAware
    {
        private string _title;

        public event EventHandler IsActiveChanged;

        /// <summary>
        /// Title
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        protected IContainerProvider ContainerProvider { get; }

        protected IEventAggregator EventAggregator { get; }

        protected IRegionManager RegionManager { get; }
        protected IDialogService DialogService { get; }

        protected IRegionNavigationService NavigationService { get; private set; }

        bool _isActive;
        /// <summary>
        /// IsActive
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive == value) return;
                SetProperty(ref _isActive, value);
                OnIsActiveChanged();
            }
        }

        private bool _canGoBack;

        public bool CanGoBack
        {
            get => _canGoBack;
            private set => SetProperty(ref _canGoBack, value);
        }

        private void OnIsActiveChanged()
        {
            IsActiveChanged?.Invoke(this, new System.EventArgs());
        }

        public ICommand GoBackCommand { get; set; }


        public ViewModelBase()
        {
        }

        public ViewModelBase(IContainerProvider containerProvider)
            : this()
        {
            ContainerProvider = containerProvider;
            EventAggregator = ContainerProvider.Resolve<IEventAggregator>();
            RegionManager = ContainerProvider.Resolve<IRegionManager>();
            DialogService = ContainerProvider.Resolve<IDialogService>();

            InitBase();
        }

        private void InitBase()
        {
            GoBackCommand = new DelegateCommand(
                () => NavigationService.Journal.GoBack(),
                () => CanGoBack)
                .ObservesCanExecute(() => CanGoBack);
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
            if(NavigationService == null 
                && navigationContext?.NavigationService != null)
            {
                NavigationService = navigationContext.NavigationService;
            }

            if(NavigationService != null)
            {
                CanGoBack = NavigationService.Journal.CanGoBack;
            }
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public virtual void Destroy()
        {

        }

        protected void SetBusy(string id, bool isBusy, [CallerMemberName] string owner = "")
        {
            EventAggregator.GetEvent<Events.BusyEvent>()
                .Publish(new EventArgs.BusyEventArgs
                {
                    Id = id,
                    IsBusy = isBusy,
                    Owner = owner
                });
        }
    }
}
