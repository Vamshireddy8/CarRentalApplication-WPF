using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using CarRentalApplication.Data;
using CarRentalApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApplication.ViewModels
{
    public class AdminDashboardViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Booking> AllBookings { get; set; }
        public ObservableCollection<Vehicle> Vehicles { get; set; }
        public Vehicle NewVehicle { get; set; } = new Vehicle();

        private Vehicle _selectedVehicle;
        public ICommand SubmitVehicleCommand { get; }
        public ICommand EditVehicleCommand { get; }
        public ICommand UpdateBookingCommand { get; }
        public ICommand ConfirmEditCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand DeleteVehicleCommand { get; }
        public ICommand ApproveBookingCommand { get; }

        private Dictionary<int, bool> _editingVehicles = new();
        public Vehicle SelectedVehicle
        {
            get => _selectedVehicle;
            set
            {
                _selectedVehicle = value;
                OnPropertyChanged(nameof(SelectedVehicle));
            }
        }

        private bool _showEditForm = false;
        public bool ShowEditForm
        {
            get => _showEditForm;
            set
            {
                _showEditForm = value;
                OnPropertyChanged(nameof(ShowEditForm));
            }
        }

        public Dictionary<int, bool> EditingVehicles
        {
            get => _editingVehicles;
            set
            {
                _editingVehicles = value;
                OnPropertyChanged(nameof(EditingVehicles));
            }
        }

        public AdminDashboardViewModel()
        {
            Vehicles = new ObservableCollection<Vehicle>();
            AllBookings = new ObservableCollection<Booking>();

            SubmitVehicleCommand = new RelayCommand(o => SubmitVehicle());
            ApproveBookingCommand = new RelayCommand(o => ApproveBooking(o as Booking));
            EditVehicleCommand = new RelayCommand(o => StartEditing(o as Vehicle));
            ConfirmEditCommand = new RelayCommand(o => ConfirmEdit(o as Vehicle));
            CancelEditCommand = new RelayCommand(o => CancelEdit());
            DeleteVehicleCommand = new RelayCommand(o => DeleteVehicle(o as Vehicle));

            LoadVehicles();
            LoadAllBookings();
        }

        private void ApproveBooking(Booking booking)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var dbBooking = db.Bookings.FirstOrDefault(b => b.Id == booking.Id);
                    if (dbBooking != null && dbBooking.Status != "Approved")
                    {
                        dbBooking.Status = "Approved";
                        db.SaveChanges();
                        LoadAllBookings();
                        booking.Status = "Approved";
                        OnPropertyChanged(nameof(AllBookings));
                        MessageBox.Show("Booking approved successfully!", "Approval Status", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error approving booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadVehicles()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var vehicles = db.Vehicles.ToList();
                    Vehicles.Clear();
                    foreach (var vehicle in vehicles)
                    {
                        Vehicles.Add(vehicle);
                        _editingVehicles[vehicle.Id] = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vehicles: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool IsEditing(Vehicle vehicle) => EditingVehicles.TryGetValue(vehicle.Id, out bool isEditing) && isEditing;

        private void DeleteVehicle(Vehicle vehicle)
        {
            try
            {
                if (vehicle == null)
                {
                    MessageBox.Show("No vehicle selected to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show($"Are you sure you want to delete {vehicle.Make} {vehicle.Model}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new AppDbContext())
                    {
                        var dbVehicle = db.Vehicles.FirstOrDefault(v => v.Id == vehicle.Id);
                        if (dbVehicle != null)
                        {
                            db.Vehicles.Remove(dbVehicle);
                            db.SaveChanges();
                            LoadVehicles();
                            MessageBox.Show("Vehicle deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Vehicle not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting vehicle: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAllBookings()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var bookings = db.Bookings.Include(b => b.Vehicle).Include(b => b.User).ToList();

                    AllBookings.Clear();
                    foreach (var booking in bookings)
                    {
                        AllBookings.Add(booking);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading bookings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SubmitVehicle()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewVehicle.Make) || string.IsNullOrWhiteSpace(NewVehicle.Model) || NewVehicle.Year == 0 || NewVehicle.Price == 0)
                {
                    MessageBox.Show("Please fill all fields.");
                    return;
                }

                using (var db = new AppDbContext())
                {
                    db.Vehicles.Add(NewVehicle);
                    db.SaveChanges();
                }

                MessageBox.Show("Vehicle added successfully!");
                LoadVehicles();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error submitting vehicle: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartEditing(Vehicle vehicle)
        {
            try
            {
                if (vehicle == null)
                {
                    MessageBox.Show("No vehicle selected for editing.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                SelectedVehicle = new Vehicle
                {
                    Id = vehicle.Id,
                    Make = vehicle.Make,
                    Model = vehicle.Model,
                    Year = vehicle.Year,
                    Price = vehicle.Price,
                    Description = vehicle.Description
                };
                ShowEditForm = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting edit: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ConfirmEdit(Vehicle vehicle)
        {
            try
            {
                if (SelectedVehicle == null)
                {
                    MessageBox.Show("No vehicle selected for update.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var db = new AppDbContext())
                {
                    var dbVehicle = db.Vehicles.FirstOrDefault(v => v.Id == SelectedVehicle.Id);
                    if (dbVehicle != null)
                    {
                        dbVehicle.Make = SelectedVehicle.Make;
                        dbVehicle.Model = SelectedVehicle.Model;
                        dbVehicle.Year = SelectedVehicle.Year;
                        dbVehicle.Price = SelectedVehicle.Price;
                        dbVehicle.Description = SelectedVehicle.Description;

                        db.SaveChanges();
                        LoadVehicles();
                        MessageBox.Show("Vehicle updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Vehicle not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                ShowEditForm = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error confirming edit: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelEdit()
        {
            ShowEditForm = false;
            SelectedVehicle = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
