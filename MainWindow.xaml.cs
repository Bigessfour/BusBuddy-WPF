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
    private ObservableCollection<Bus> _buses = new();
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
    public ObservableCollection<Bus> Buses
    {
        get => _buses;
        set { _buses = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Buses))); }
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
            .Include(r => r.AMBus)
            .Include(r => r.AMDriver)
            .Include(r => r.PMBus)
            .Include(r => r.PMDriver)
            .ToList());
        Drivers = new ObservableCollection<Driver>(_context.Drivers.ToList());
        Buses = new ObservableCollection<Bus>(_context.Buses.ToList());
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