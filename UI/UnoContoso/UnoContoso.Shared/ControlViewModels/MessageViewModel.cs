using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using UnoContoso.Base;

namespace UnoContoso.ControlViewModels
{
    public class MessageViewModel : DialogViewModelBase
    {
        protected override void OnClose(string obj)
        {
            RaiseRequestClose(new DialogResult(ButtonResult.OK));
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            parameters.TryGetValue("title", out string title);
            Title = title ?? "Confirmation";
            Message = parameters.GetValue<string>("message");
        }
    }
}
