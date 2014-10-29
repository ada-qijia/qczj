using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace AutoWP7.UcControl
{
    public class ImageToggleButton : ContentControl
    {
        #region Property

        public ImageSource CheckedBackground
        {
            get { return (ImageSource)GetValue(CheckedBackgroundProperty); }
            set
            {
                SetValue(CheckedBackgroundProperty, value);
            }
        }

        public static readonly DependencyProperty CheckedBackgroundProperty =
            DependencyProperty.Register("CheckedBackground", typeof(ImageSource), typeof(ImageToggleButton), new PropertyMetadata(null));

        public ImageSource UnCheckedBackground
        {
            get { return (ImageSource)GetValue(UnCheckedBackgroundProperty); }
            set
            {
                SetValue(UnCheckedBackgroundProperty, value);
            }
        }

        public static readonly DependencyProperty UnCheckedBackgroundProperty =
            DependencyProperty.Register("UnCheckedBackground", typeof(ImageSource), typeof(ImageToggleButton), new PropertyMetadata(null));

        public bool Checked
        {
            get { return (bool)GetValue(CheckedProperty); }
            set
            {
                SetValue(CheckedProperty, value);
            }
        }

        public static readonly DependencyProperty CheckedProperty =
            DependencyProperty.Register("Checked", typeof(bool), typeof(ImageToggleButton), new PropertyMetadata(false, OnCheckedPropertyChanged));

        private static void OnCheckedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageToggleButton control = d as ImageToggleButton;
            bool newValue = (bool)e.NewValue;
            if (newValue)
            {
                VisualStateManager.GoToState(control, "Checked", true);
            }
            else
            {
                VisualStateManager.GoToState(control, "UnChecked", true);
            }
        }

        #endregion

        #region Constructor

        void ImageToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "UnChecked", false);
        }

        void ImageToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Checked", false);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.Checked == true)
            {
                VisualStateManager.GoToState(this, "Checked", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "UnChecked", false);
            }
        }

        #endregion

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            VisualStateManager.GoToState(this, "Pressed", true);
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            GoNormal();
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            GoNormal();
        }

        private void GoNormal()
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        //protected override void OnTap(System.Windows.Input.GestureEventArgs e)
        //{
        //    base.OnTap(e);
        //    this.Checked = !this.Checked;
        //}

    }
}
