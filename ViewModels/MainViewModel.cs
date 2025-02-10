using System;
using System.ComponentModel;
using System.Windows.Input;
using CarRentalApplication.Helpers;

namespace CarRentalApplication.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private static MainViewModel _instance;
        public static MainViewModel Instance => _instance ??= new MainViewModel();

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        private bool _isAdmin = false;
        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
                OnPropertyChanged(nameof(IsAdmin));
            }
        }

        private bool _isLoggedIn = false;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                if (_isLoggedIn != value)
                {
                    _isLoggedIn = value;
                    OnPropertyChanged(nameof(IsLoggedIn));
                    OnPropertyChanged(nameof(IsLoggedOut));
                }
            }
        }

        public bool IsLoggedOut => !IsLoggedIn;

        private string _welcomeMessage = "";
        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set
            {
                if (_welcomeMessage != value)
                {
                    _welcomeMessage = value;
                    OnPropertyChanged(nameof(WelcomeMessage));
                }
            }
        }

        public ICommand NavigateHome { get; }
        public ICommand NavigateBookingSystem { get; }
        public ICommand NavigateProfile { get; }
        public ICommand NavigateContact { get; }
        public ICommand NavigateRentalTerms { get; }
        public ICommand NavigateAdminDashboard { get; }
        public ICommand NavigateLogin { get; }
        public ICommand NavigateRegister { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel()
        {
            try
            {
                NavigateHome = new RelayCommand(o => NavigateToView(new HomePageViewModel()));
                NavigateBookingSystem = new RelayCommand(o => NavigateToView(new BookingSystemViewModel()));
                NavigateProfile = new RelayCommand(o => NavigateToView(new ProfileViewModel()));
                NavigateContact = new RelayCommand(o => NavigateToView(new ContactViewModel()));
                NavigateRentalTerms = new RelayCommand(o => NavigateToView(new RentalTermsViewModel()));
                NavigateAdminDashboard = new RelayCommand(o => NavigateToView(new AdminDashboardViewModel()));
                NavigateLogin = new RelayCommand(o => NavigateToView(new LoginViewModel()));
                NavigateRegister = new RelayCommand(o => NavigateToView(new RegisterViewModel()));
                LogoutCommand = new RelayCommand(o => Logout());

                _currentView = new HomePageViewModel();
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while initializing MainViewModel.", ex);
            }
        }

        private void NavigateToView(object viewModel)
        {
            try
            {
                CurrentView = viewModel;
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while navigating between views.", ex);
            }
        }

        private void Logout()
        {
            try
            {
                UserSession.Logout();
                IsLoggedIn = false;
                IsAdmin = false;
                WelcomeMessage = "";
                CurrentView = new HomePageViewModel();
                OnPropertyChanged(nameof(CurrentView));
            }
            catch (Exception ex)
            {
                HandleError("An error occurred during logout.", ex);
            }
        }

        private void HandleError(string message, Exception ex)
        {
            Console.WriteLine($"{message} Error: {ex.Message}");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {
                HandleError($"An error occurred while notifying property change: {propertyName}.", ex);
            }
        }
    }
}
