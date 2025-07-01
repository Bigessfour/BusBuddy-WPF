using System.Collections.ObjectModel;
using System.Windows.Input;
using BusBuddy.Models;

namespace BusBuddy.ViewModels
{
    public class BusesViewModel
    {
        public ObservableCollection<Bus> Buses { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public BusesViewModel()
        {
            // TODO: Replace with EF Core or repository data access
            Buses = new ObservableCollection<Bus>();
            AddCommand = new RelayCommand(AddBus);
            EditCommand = new RelayCommand(EditBus, CanEditOrDelete);
            DeleteCommand = new RelayCommand(DeleteBus, CanEditOrDelete);
        }
        private void AddBus(object? obj) { /* Open dialog for Add */ }
        private void EditBus(object? obj) { /* Open dialog for Edit */ }
        private void DeleteBus(object? obj) { /* Remove from collection and data source */ }
        private bool CanEditOrDelete(object? obj) => true;
    }
}
