using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using CarRentalApplication.Data;
using CarRentalApplication.Helpers;
using CarRentalApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApplication.ViewModels
{
    public class BookingSystemViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Vehicle> AvailableVehicles { get; set; }
        private Vehicle _selectedVehicle;
        public Vehicle SelectedVehicle
        {
            get => _selectedVehicle;
            set
            {
                _selectedVehicle = value;
                OnPropertyChanged(nameof(SelectedVehicle));
            }
        }

        private DateTime _startDate = DateTime.Today;
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        private DateTime _endDate = DateTime.Today.AddDays(1);
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }

        private Booking _selectedBooking;
        public Booking SelectedBooking
        {
            get => _selectedBooking;
            set
            {
                _selectedBooking = value;
                OnPropertyChanged(nameof(SelectedBooking));
                ShowUpdateForm = _selectedBooking != null;
            }
        }

        private bool _showUpdateForm;
        public bool ShowUpdateForm
        {
            get => _showUpdateForm;
            set
            {
                _showUpdateForm = value;
                OnPropertyChanged(nameof(ShowUpdateForm));
            }
        }

        public ObservableCollection<Booking> UserBookings { get; set; }

        public ICommand BookVehicleCommand { get; }
        public ICommand LoadUserBookingsCommand { get; }
        public ICommand CancelBookingCommand { get; }

        public BookingSystemViewModel()
        {
            UserBookings = new ObservableCollection<Booking>();
            AvailableVehicles = new ObservableCollection<Vehicle>();
            BookVehicleCommand = new RelayCommand(o => BookVehicle());
            LoadUserBookingsCommand = new RelayCommand(o => LoadUserBookings());
            CancelBookingCommand = new RelayCommand(o => CancelBooking(o as Booking));

            LoadAvailableVehicles();
            LoadUserBookings();
        }

        private void LoadAvailableVehicles()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    AvailableVehicles.Clear();
                    foreach (var vehicle in db.Vehicles.ToList())
                    {
                        AvailableVehicles.Add(vehicle);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vehicles: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BookVehicle()
        {
            try
            {
                if (SelectedVehicle == null)
                {
                    MessageBox.Show("Please select a vehicle.");
                    return;
                }

                if (StartDate >= EndDate)
                {
                    MessageBox.Show("End Date must be after Start Date.");
                    return;
                }

                using (var db = new AppDbContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.Id == UserSession.UserId);
                    if (user == null)
                    {
                        MessageBox.Show("User not found. Please log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var newBooking = new Booking
                    {
                        UserId = UserSession.GetCurrentUserId(),
                        VehicleId = SelectedVehicle.Id,
                        StartDate = StartDate.ToShortDateString(),
                        EndDate = EndDate.ToShortDateString(),
                        Status = "Pending"
                    };

                    db.Bookings.Add(newBooking);
                    db.SaveChanges();

                    MessageBox.Show("Booking successful! Waiting for admin approval.", "Booking Status", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadUserBookings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error booking vehicle: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CancelBooking(Booking booking)
        {
            try
            {
                if (booking == null)
                {
                    MessageBox.Show("No booking selected to cancel.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var db = new AppDbContext())
                {
                    var dbBooking = db.Bookings.FirstOrDefault(b => b.Id == booking.Id && b.UserId == UserSession.UserId);
                    if (dbBooking != null)
                    {
                        db.Bookings.Remove(dbBooking);
                        db.SaveChanges();
                        LoadUserBookings();
                        MessageBox.Show("Booking canceled successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("You can only cancel your own bookings.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error canceling booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadUserBookings()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var userId = UserSession.GetCurrentUserId();
                    var userBookings = db.Bookings
                        .Where(b => b.UserId == userId)
                        .Include(b => b.Vehicle)
                        .ToList();

                    UserBookings.Clear();
                    foreach (var booking in userBookings)
                    {
                        UserBookings.Add(booking);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user bookings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
