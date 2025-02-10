using System.Windows.Input;
using CarRentalApplication.Helpers;
using CarRentalApplication.Models;

namespace CarRentalApplication.ViewModels
{
    public class HomePageViewModel
    {
        public ICommand NavigateLogin {  get; }

        public HomePageViewModel()
        {
            if(UserSession.UserId>0)
            {
                switch (UserSession.CurrentUserRole)
                {
                    case "Admin":
                        NavigateLogin = new RelayCommand(o => MainViewModel.Instance.NavigateAdminDashboard.Execute(null));
                        break;

                    case "User":
                        NavigateLogin = new RelayCommand(o => MainViewModel.Instance.NavigateBookingSystem.Execute(null));
                        break;

                    default:
                        NavigateLogin = new RelayCommand(o => MainViewModel.Instance.NavigateLogin.Execute(null));
                        break;
                }
            }
            else
            {
                NavigateLogin = new RelayCommand(o => MainViewModel.Instance.NavigateLogin.Execute(null));

            }


        }
    }
}
