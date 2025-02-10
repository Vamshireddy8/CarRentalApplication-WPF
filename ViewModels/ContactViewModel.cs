using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace CarRentalApplication.ViewModels
{
    public class ContactViewModel : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
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

        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public ICommand SendMessageCommand { get; }

        public ContactViewModel()
        {
            SendMessageCommand = new RelayCommand(o => SendMessage());
        }

        private void SendMessage()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Message))
                {
                    MessageBox.Show("Please fill out all fields before sending.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBox.Show($"Message Sent!\n\nName: {Name}\nEmail: {Email}\nMessage: {Message}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                Name = string.Empty;
                Email = string.Empty;
                Message = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while sending the message: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
