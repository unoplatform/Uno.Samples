using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
//using Uno.Extensions.Specialized;
using UnoContoso.Base;

namespace UnoContoso.ControlViewModels
{
    public class ConfirmViewModel : DialogViewModelBase
    {
        private string _button0Text;
        public string Button0Text
        {
            get { return _button0Text; }
            set { SetProperty(ref _button0Text, value); }
        }

        private string _button1Text;
        public string Button1Text
        {
            get { return _button1Text; }
            set { SetProperty(ref _button1Text, value); }
        }

        private string _button2Text;
        public string Button2Text
        {
            get { return _button2Text; }
            set { SetProperty(ref _button2Text, value); }
        }

        protected override void OnClose(string obj)
        {
            ButtonResult result = ButtonResult.None;
            switch(obj.ToLower())
            {
                case "0":
                    result = ButtonResult.Yes;
                    break;
                case "1":
                    result = ButtonResult.No;
                    break;
                case "2":
                    result = ButtonResult.Cancel;
                    break;
            }
            RaiseRequestClose(new DialogResult(result));
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            parameters.TryGetValue("title", out string title);
            Title = title ?? "Confirmation";
            Message = parameters.GetValue<string>("message");
            var buttons = parameters.GetValue<string>("buttons");
            if (string.IsNullOrEmpty(buttons))
            {
                Button0Text = "Save";
                Button1Text = "Don't save";
                Button2Text = "Cancel";
            }
            else
            {
                var b = buttons.Split(',');
                for (int i = 0; i < b.Length; i++)
                {
                    if (i == 0) Button0Text = b[i];
                    if (i == 1) Button1Text = b[i];
                    if (i == 2) 
                    { 
                        Button2Text = b[i]; 
                    }
                }
            }
        }
    }
}
