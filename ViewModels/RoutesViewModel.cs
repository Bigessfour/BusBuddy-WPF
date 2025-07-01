using System.Collections.ObjectModel;
using System.Windows.Input;
using BusBuddy.Models;

namespace BusBuddy.ViewModels
{
    public class RoutesViewModel
    {
        public ObservableCollection<Route> Routes { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public RoutesViewModel()
        {
            // Load data using repository or EF Core
            Routes = new ObservableCollection<Route>();
            AddCommand = new RelayCommand(AddRoute);
            EditCommand = new RelayCommand(EditRoute, CanEditOrDelete);
            DeleteCommand = new RelayCommand(DeleteRoute, CanEditOrDelete);
        }
        private void AddRoute(object? obj) { /* Open dialog for Add */ }
        private void EditRoute(object? obj) { /* Open dialog for Edit */ }
        private void DeleteRoute(object? obj) { /* Remove from collection and data source */ }
        private bool CanEditOrDelete(object? obj) => true;
    }
}
