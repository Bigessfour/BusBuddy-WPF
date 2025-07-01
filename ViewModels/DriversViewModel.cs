using System.Collections.ObjectModel;
using System.Windows.Input;
using BusBuddy.Models;

namespace BusBuddy.ViewModels
{
    public class DriversViewModel
    {
        public ObservableCollection<Driver> Drivers { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public DriversViewModel()
        {
            Drivers = new ObservableCollection<Driver>();
            AddCommand = new RelayCommand(AddDriver);
            EditCommand = new RelayCommand(EditDriver, CanEditOrDelete);
            DeleteCommand = new RelayCommand(DeleteDriver, CanEditOrDelete);
        }
        private void AddDriver(object? obj) { /* Open dialog for Add */ }
        private void EditDriver(object? obj) { /* Open dialog for Edit */ }
        private void DeleteDriver(object? obj) { /* Remove from collection and data source */ }
        private bool CanEditOrDelete(object? obj) => true;
    }
}
