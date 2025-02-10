using System.Windows;
using System.Windows.Controls;

namespace CarRentalApplication.Helpers
{
    public static class PasswordBoxHelper
    {
        private static bool _isUpdating = false;

        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxHelper),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundPasswordChanged));

        public static readonly DependencyProperty BindPasswordProperty =
            DependencyProperty.RegisterAttached("BindPassword", typeof(bool), typeof(PasswordBoxHelper),
                new PropertyMetadata(false, OnBindPasswordChanged));

        public static void SetBoundPassword(DependencyObject obj, string value)
        {
            obj.SetValue(BoundPasswordProperty, value);
        }

        public static string GetBoundPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(BoundPasswordProperty);
        }

        public static void SetBindPassword(DependencyObject obj, bool value)
        {
            obj.SetValue(BindPasswordProperty, value);
        }

        public static bool GetBindPassword(DependencyObject obj)
        {
            return (bool)obj.GetValue(BindPasswordProperty);
        }

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (_isUpdating) return; 

            if (d is PasswordBox passwordBox)
            {
                _isUpdating = true;
                if (!string.Equals(passwordBox.Password, (string)e.NewValue))
                {
                    passwordBox.Password = (string)e.NewValue;
                }
                _isUpdating = false;
            }
        }

        private static void OnBindPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                if ((bool)e.NewValue)
                {
                    passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                }
                else
                {
                    passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                }
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_isUpdating) return;

            if (sender is PasswordBox passwordBox)
            {
                _isUpdating = true;
                SetBoundPassword(passwordBox, passwordBox.Password);
                _isUpdating = false;
            }
        }
    }
}
