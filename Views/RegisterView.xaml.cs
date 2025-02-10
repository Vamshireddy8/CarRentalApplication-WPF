using System.Windows;
using System.Windows.Controls;

namespace CarRentalApplication.Views
{
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.RegisterViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}
