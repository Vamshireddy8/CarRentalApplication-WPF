using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace CarRentalApplication.ViewModels
{
    public class RentalTermsViewModel : INotifyPropertyChanged
    {
        public ICommand NavigateToContact { get; }

        public ObservableCollection<RentalTerm> RentalTerms { get; set; }

        public RentalTermsViewModel()
        {
            try
            {
                LoadRentalTerms();
                NavigateToContact = new RelayCommand(o => NavigateToContactView());
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while initializing RentalTermsViewModel.", ex);
            }
        }

        private void NavigateToContactView()
        {
            MainViewModel.Instance.CurrentView = new ContactViewModel();
        }

        private void LoadRentalTerms()
        {
            try
            {
                RentalTerms = new ObservableCollection<RentalTerm>
                {
                    new RentalTerm { Title = "1. Eligibility", Description = "To rent a vehicle, you must be at least 21 years old and have a valid driver's license. A credit card is required for payment and security deposit." },
                    new RentalTerm { Title = "2. Reservation and Payment", Description = "Reservations can be made online or in person. A valid credit card is required to secure your reservation. Full payment is due at pickup." },
                    new RentalTerm { Title = "3. Fuel Policy", Description = "Vehicles must be returned with the same amount of fuel as at pickup. Failure to do so may result in additional fuel charges." },
                    new RentalTerm { Title = "4. Insurance Coverage", Description = "Insurance coverage options are available at rental. It is highly recommended to purchase insurance to protect against damage or theft." },
                    new RentalTerm { Title = "5. Mileage Policy", Description = "Mileage limits apply to certain rentals. Please check your rental agreement for details on mileage charges." },
                    new RentalTerm { Title = "6. Cancellation Policy", Description = "Cancellations must be made at least 24 hours in advance to avoid charges. Please refer to our website for specific cancellation policies." },
                    new RentalTerm { Title = "7. Contact Us", Description = "If you have any questions regarding the rental terms and conditions, please feel free to contact our support team." }
                };

                OnPropertyChanged(nameof(RentalTerms));
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while loading rental terms.", ex);
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

    public class RentalTerm
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
