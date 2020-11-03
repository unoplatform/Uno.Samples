using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;

namespace UnoContoso.UserControls
{
    /// <summary>
    /// Temporarily created and used since the content by ElementName cannot be found in the UserControl that is currently output using DialogService.
    /// </summary>
    public partial class ViewModelElement : FrameworkElement
    {
        #region ViewModel
        public object ViewModel
        {
            get => GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(object), typeof(ViewModelElement), new PropertyMetadata(null));
        #endregion
    }
}
