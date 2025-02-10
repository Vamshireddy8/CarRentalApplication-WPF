using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using CarRentalApplication.Data;
using CarRentalApplication.Helpers;
using CarRentalApplication.Models;
using CarRentalApplication.ViewModels;

namespace CarRentalApplication.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand NavigateRegisterCommand { get; }

        public event Action OnLoginSuccess; // Event to trigger navigation after login

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(o => Login());
            NavigateRegisterCommand = new RelayCommand(o => MainViewModel.Instance.CurrentView = new RegisterViewModel());

        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private void Login()
        {
            try
            {
                if (!IsValidEmail(Email))
                {
                    MessageBox.Show("Invalid email format. Please enter a valid email.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    MessageBox.Show("Please enter both Email and Password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var db = new AppDbContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.Email == Email);
                    if (user != null && VerifyPassword(Password, user.Password))
                    {
                        bool isAdminUser = user.Role == "Admin";

                        UserSession.LoginAsUser(user.FullName, user.Id);
                        MainViewModel.Instance.IsLoggedIn = true;
                        MainViewModel.Instance.IsAdmin = isAdminUser;
                        MainViewModel.Instance.WelcomeMessage = $"Welcome, {user.FullName}!";

                        if (!isAdminUser)
                        {
                            MainViewModel.Instance.CurrentView = new BookingSystemViewModel();
                        }
                        else
                        {
                            MainViewModel.Instance.CurrentView = new AdminDashboardViewModel();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid Email or Password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during login: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            try
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputPassword));
                    StringBuilder builder = new StringBuilder();
                    foreach (byte b in bytes)
                    {
                        builder.Append(b.ToString("x2"));
                    }
                    return builder.ToString() == storedHashedPassword;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while verifying the password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        public static void ResetAdminPassword()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var admin = db.Users.FirstOrDefault(u => u.Email == "admin@carrental.com");
                    if (admin != null)
                    {
                        admin.Password = HashPassword("admin123");
                        db.SaveChanges();
                        MessageBox.Show("Admin password has been reset successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Admin account not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while resetting the admin password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string HashPassword(string password)
        {
            try
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    StringBuilder builder = new StringBuilder();
                    foreach (byte b in bytes)
                    {
                        builder.Append(b.ToString("x2"));
                    }
                    return builder.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while hashing the password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
