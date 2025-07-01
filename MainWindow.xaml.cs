using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using BusBuddy.Data;
using BusBuddy.Models;

namespace BusBuddy;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    private ObservableCollection<Route> _routes = new();
    private ObservableCollection<Driver> _drivers = new();
    private ObservableCollection<Vehicle> _vehicles = new();
    public ObservableCollection<Route> Routes
    {
        get => _routes;
        set { _routes = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Routes))); }
    }
    public ObservableCollection<Driver> Drivers
    {
        get => _drivers;
        set { _drivers = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Drivers))); }
    }
    public ObservableCollection<Vehicle> Vehicles
    {
        get => _vehicles;
        set { _vehicles = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Vehicles))); }
    }
    public ObservableCollection<string> RouteNames { get; set; } = new ObservableCollection<string> { "Truck Plaza", "East Route", "West Route", "SPED" };
    private readonly BusBuddyContext _context;
    public event PropertyChangedEventHandler? PropertyChanged;
    public MainWindow()
    {
        InitializeComponent();
        _context = new BusBuddyContext();
        DataContext = this;
        LoadData();
    }
    private void LoadData()
    {
        Routes = new ObservableCollection<Route>(_context.Routes
            .Include(r => r.AMVehicle)
            .Include(r => r.AMDriver)
            .Include(r => r.PMVehicle)
            .Include(r => r.PMDriver)
            .ToList());
        Drivers = new ObservableCollection<Driver>(_context.Drivers.ToList());
        Vehicles = new ObservableCollection<Vehicle>(_context.Vehicles.ToList());
    }
    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _context.Routes.RemoveRange(_context.Routes);
            _context.Routes.AddRange(Routes);
            _context.SaveChanges();
            MessageBox.Show("Changes saved!");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving changes: {ex.Message}");
        }
    }
}