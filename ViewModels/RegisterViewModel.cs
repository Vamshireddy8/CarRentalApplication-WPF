using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using CarRentalApplication.Data;
using CarRentalApplication.Models;

namespace CarRentalApplication.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

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

        public ICommand RegisterCommand { get; }

        public RegisterViewModel()
        {
            try
            {
                RegisterCommand = new RelayCommand(o => Register());
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while initializing the RegisterViewModel.", ex);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while validating email.", ex);
                return false;
            }
        }

        private bool IsValidPassword(string password)
        {
            try
            {
                return password.Length >= 8 &&
                       Regex.IsMatch(password, @"[A-Z]") &&
                       Regex.IsMatch(password, @"[0-9]") &&
                       Regex.IsMatch(password, @"[\W]");
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while validating password.", ex);
                return false;
            }
        }

        public void Register()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    if (!IsValidEmail(Email))
                    {
                        MessageBox.Show("Invalid email format. Please enter a valid email.");
                        return;
                    }

                    if (!IsValidPassword(Password))
                    {
                        MessageBox.Show("Password must be at least 8 characters long and contain at least one uppercase letter, one number, and one special character.");
                        return;
                    }

                    var existingUser = db.Users.FirstOrDefault(u => u.Email == Email);
                    if (existingUser != null)
                    {
                        MessageBox.Show("This email is already registered.");
                        return;
                    }

                    var newUser = new User
                    {
                        FullName = FullName,
                        Email = Email,
                        Password = HashPassword(Password),
                        Role = "User"
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();

                    MessageBox.Show("Registration successful! Please log in.");
                    MainViewModel.Instance.CurrentView = new LoginViewModel();
                }
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while registering the user.", ex);
            }
        }

        private string HashPassword(string password)
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
                HandleError("An error occurred while hashing the password.", ex);
                return string.Empty;
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
