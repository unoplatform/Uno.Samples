using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace UnoContoso.Base
{
    public abstract class DialogViewModelBase : BindableBase, IDialogAware
    {
        public ICommand CloseDialogCommand { get; set; }

        #region Title
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        #endregion

        #region Message
        private string _messge;

        public string Message
        {
            get { return _messge; }
            set { SetProperty(ref _messge, value); }
        }
        #endregion

        public DialogViewModelBase()
        {
            Init();
        }

        protected virtual void Init()
        {
            CloseDialogCommand = new DelegateCommand<string>(OnClose);
        }

        protected virtual void OnClose(string obj)
        {
        }

        public event Action<IDialogResult> RequestClose;

        protected void RaiseRequestClose(DialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {
        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
