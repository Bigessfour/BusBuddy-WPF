using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using BusBuddy.Core.Models;
using BusBuddy.Core.Services;
using BusBuddy.Core.Services.Interfaces;
using Serilog;
using Serilog.Context;
using CoreModels = BusBuddy.Core.Models;

namespace BusBuddy.WPF.Views.Fuel
{
    public partial class FuelDialog : Window, INotifyPropertyChanged
    {
        private static readonly ILogger Logger = Log.ForContext<FuelDialog>();

        private readonly IBusService _busService;
        private CoreModels.Fuel _fuel;
        private CoreModels.Bus? _selectedBus;
        private bool _isValid = false;
        private double _mpg;
        private bool _isUpdatingCost = false;

        // Observable collections for ComboBox options
        public ObservableCollection<CoreModels.Bus> AvailableBuses { get; } = new ObservableCollection<CoreModels.Bus>();
        public ObservableCollection<string> FuelLocations { get; } = new ObservableCollection<string>
        {
            "Key Pumps", "School District Pump", "BP", "Shell", "Chevron", "Exxon", "Mobil", "Marathon", "Sunoco", "Other"
        };
        public ObservableCollection<string> FuelTypes { get; } = new ObservableCollection<string>
        {
            "Gasoline", "Diesel", "Biodiesel", "CNG", "Propane"
        };

        public string DialogTitle { get; private set; }

        public CoreModels.Fuel Fuel
        {
            get => _fuel;
            set
            {
                _fuel = value;
                OnPropertyChanged();
                CalculateMPG();
                ValidateForm();
            }
        }

        public CoreModels.Bus? SelectedBus
        {
            get => _selectedBus;
            set
            {
                _selectedBus = value;
                if (_selectedBus != null)
                {
                    Fuel.VehicleFueledId = _selectedBus.VehicleId;
                }
                OnPropertyChanged();
                ValidateForm();
            }
        }

        public bool IsValid
        {
            get => _isValid;
            set
            {
                _isValid = value;
                OnPropertyChanged();
            }
        }

        public double MPG
        {
            get => _mpg;
            set
            {
                _mpg = value;
                OnPropertyChanged();
            }
        }

        public FuelDialog(CoreModels.Fuel fuel, IBusService busService)
        {
            _fuel = fuel ?? throw new ArgumentNullException(nameof(fuel));
            _busService = busService ?? throw new ArgumentNullException(nameof(busService));

            DialogTitle = fuel.FuelId == 0 ? "Add Fuel Record" : "Edit Fuel Record";

            InitializeComponent();
            DataContext = this;

            // Ensure the model has default values if it's new
            if (fuel.FuelId == 0)
            {
                fuel.FuelDate = DateTime.Now;
                fuel.FuelType = "Diesel"; // Default for school buses
                fuel.FuelLocation = "Key Pumps"; // Default location
            }

            LoadBusesAsync();
        }

        private async void LoadBusesAsync()
        {
            try
            {
                AvailableBuses.Clear();
                var buses = await _busService.GetAllBusesAsync();
                foreach (var bus in buses)
                {
                    AvailableBuses.Add(bus);
                }

                // Select the bus that matches the fuel record
                _selectedBus = AvailableBuses.FirstOrDefault(b => b.VehicleId == Fuel.VehicleFueledId);
                OnPropertyChanged(nameof(SelectedBus));

                using (LogContext.PushProperty("ViewType", "FuelDialog"))
                using (LogContext.PushProperty("OperationType", "LoadBuses"))
                {
                    Logger.Information("Loaded {BusCount} buses for fuel dialog", AvailableBuses.Count);
                }
            }
            catch (Exception ex)
            {
                using (LogContext.PushProperty("ViewType", "FuelDialog"))
                using (LogContext.PushProperty("OperationType", "LoadBuses"))
                {
                    Logger.Error(ex, "Error loading buses for fuel dialog");
                }
                MessageBox.Show($"Error loading buses: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ValidateForm()
        {
            IsValid =
                SelectedBus != null &&
                !string.IsNullOrWhiteSpace(Fuel.FuelLocation) &&
                !string.IsNullOrWhiteSpace(Fuel.FuelType) &&
                Fuel.Gallons > 0;
        }

        private void CalculateMPG()
        {
            if (Fuel.Gallons.HasValue && Fuel.Gallons.Value > 0)
            {
                // Simple calculation for demonstration purposes
                // In a real app, we would need to retrieve the previous odometer reading
                // for this vehicle to calculate actual MPG
                MPG = 0; // Placeholder

                // Auto-calculate total cost if price per gallon is set
                if (!_isUpdatingCost && Fuel.PricePerGallon.HasValue && Fuel.PricePerGallon.Value > 0)
                {
                    _isUpdatingCost = true;
                    Fuel.TotalCost = Math.Round(Fuel.Gallons.Value * Fuel.PricePerGallon.Value, 2);
                    OnPropertyChanged(nameof(Fuel.TotalCost));
                    _isUpdatingCost = false;
                }
            }
            else
            {
                MPG = 0;
            }
        }

        private void PricePerGallon_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (_isUpdatingCost) return;

            _isUpdatingCost = true;
            if (Fuel.Gallons.HasValue && Fuel.Gallons.Value > 0 && Fuel.PricePerGallon.HasValue)
            {
                Fuel.TotalCost = Math.Round(Fuel.Gallons.Value * Fuel.PricePerGallon.Value, 2);
                OnPropertyChanged(nameof(Fuel.TotalCost));
            }
            _isUpdatingCost = false;
        }

        private void TotalCost_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (_isUpdatingCost) return;

            _isUpdatingCost = true;
            if (Fuel.Gallons.HasValue && Fuel.Gallons.Value > 0 && Fuel.TotalCost.HasValue)
            {
                Fuel.PricePerGallon = Math.Round(Fuel.TotalCost.Value / Fuel.Gallons.Value, 3);
                OnPropertyChanged(nameof(Fuel.PricePerGallon));
            }
            _isUpdatingCost = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
