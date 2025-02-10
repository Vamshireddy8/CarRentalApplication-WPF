using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CarRentalApplication.Data;
using CarRentalApplication.Models;
using CarRentalApplication.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApplication.ViewModels
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        private User _user;
        public User User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged(nameof(User));
            }
        }

        public ObservableCollection<Booking> Bookings { get; set; }

        public ProfileViewModel()
        {
            try
            {
                LoadUserData();
                LoadBookings();
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while initializing the ProfileViewModel.", ex);
            }
        }

        private void LoadUserData()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var userId = UserSession.GetCurrentUserId();
                    var user = db.Users.FirstOrDefault(u => u.Id == userId);

                    if (user != null)
                    {
                        User = new User
                        {
                            FullName = user.FullName,
                            Email = user.Email
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while loading user data.", ex);
            }
        }

        private void LoadBookings()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var userId = UserSession.GetCurrentUserId();
                    var userBookings = db.Bookings.Where(b => b.UserId == userId).Include(b => b.Vehicle).ToList();

                    Bookings = new ObservableCollection<Booking>(userBookings);
                    OnPropertyChanged(nameof(Bookings));
                }
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while loading user bookings.", ex);
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
