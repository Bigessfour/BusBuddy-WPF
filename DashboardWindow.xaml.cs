using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BusBuddy
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow()
        {
            InitializeComponent();
            // Initialize with Routes tab visible
            ShowRoutesTab();
        }

        private void Routes_Click(object sender, RoutedEventArgs e)
        {
            ShowRoutesTab();
            UpdateButtonStyles(RoutesButton);
        }

        private void Buses_Click(object sender, RoutedEventArgs e)
        {
            ShowBusesTab();
            UpdateButtonStyles(BusesButton);
        }

        private void Drivers_Click(object sender, RoutedEventArgs e)
        {
            ShowDriversTab();
            UpdateButtonStyles(DriversButton);
        }

        private void ShowRoutesTab()
        {
            MainTabControl.SelectedItem = RoutesTab;
            RoutesTab.Visibility = Visibility.Visible;
            BusesTab.Visibility = Visibility.Collapsed;
            DriversTab.Visibility = Visibility.Collapsed;
        }

        private void ShowBusesTab()
        {
            MainTabControl.SelectedItem = BusesTab;
            RoutesTab.Visibility = Visibility.Collapsed;
            BusesTab.Visibility = Visibility.Visible;
            DriversTab.Visibility = Visibility.Collapsed;
        }

        private void ShowDriversTab()
        {
            MainTabControl.SelectedItem = DriversTab;
            RoutesTab.Visibility = Visibility.Collapsed;
            BusesTab.Visibility = Visibility.Collapsed;
            DriversTab.Visibility = Visibility.Visible;
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
