using CarRentalApplication.ViewModels;
using System.Windows;

namespace CarRentalApplication
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); 
            DataContext = new ViewModels.MainViewModel(); 
            DataContext = MainViewModel.Instance;
        }
    }
}
