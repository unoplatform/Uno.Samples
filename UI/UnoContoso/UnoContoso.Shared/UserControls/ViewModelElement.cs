using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;

namespace UnoContoso.UserControls
{
    /// <summary>
    /// 현재 DialogService를 이용해서 출력한 UserControl에서 ElementName으로 내용을 찾을 수 없기 때문에 임시로 만들어 사용
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
