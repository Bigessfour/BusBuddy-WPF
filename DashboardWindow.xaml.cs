using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BusBuddy.Views;

namespace BusBuddy
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow()
        {
            InitializeComponent();
            // Initialize with Routes view
            ShowRoutesView();
            UpdateButtonStyles(RoutesButton);
        }

        private void Routes_Click(object sender, RoutedEventArgs e)
        {
            ShowRoutesView();
            UpdateButtonStyles(RoutesButton);
        }

        private void Buses_Click(object sender, RoutedEventArgs e)
        {
            ShowBusesView();
            UpdateButtonStyles(BusesButton);
        }

        private void Drivers_Click(object sender, RoutedEventArgs e)
        {
            ShowDriversView();
            UpdateButtonStyles(DriversButton);
        }

        private void ShowRoutesView()
        {
            MainContent.Content = new RoutesManagementView();
            HeaderText.Text = "Routes Management";
        }

        private void ShowBusesView()
        {
            MainContent.Content = new BusesManagementView();
            HeaderText.Text = "Bus Fleet Management";
        }

        private void ShowDriversView()
        {
            MainContent.Content = new DriversManagementView();
            HeaderText.Text = "Driver Management";
        }

        private void UpdateButtonStyles(Button activeButton)
        {
            // Reset all buttons to default style
            RoutesButton.Background = new SolidColorBrush(Color.FromRgb(74, 85, 104));
            BusesButton.Background = new SolidColorBrush(Color.FromRgb(74, 85, 104));
            DriversButton.Background = new SolidColorBrush(Color.FromRgb(74, 85, 104));

            // Highlight active button
            activeButton.Background = new SolidColorBrush(Color.FromRgb(56, 178, 172));
        }
    }
}
