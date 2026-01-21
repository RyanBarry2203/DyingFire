using System.Windows;
using DyingFire.ViewModels;

namespace DyingFire
{
    public partial class MainWindow : Window
    {
        // We need access to the ViewModel to change data
        private MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();

            // Connect the ViewModel
            _vm = new MainViewModel();
            this.DataContext = _vm;
        }

        // --- NAVIGATION BUTTONS ---
        private void MoveNorth_Click(object sender, RoutedEventArgs e) { _vm.Move("North"); }
        private void MoveSouth_Click(object sender, RoutedEventArgs e) { _vm.Move("South"); }
        private void MoveWest_Click(object sender, RoutedEventArgs e) { _vm.Move("West"); }
        private void MoveEast_Click(object sender, RoutedEventArgs e) { _vm.Move("East"); }

        // --- INVENTORY BUTTON ---
        private void Inventory_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Opening Inventory...");
        }
    }
}