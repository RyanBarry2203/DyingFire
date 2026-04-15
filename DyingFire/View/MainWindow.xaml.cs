using System.Windows;
using System.Windows.Input;
using DyingFire.ViewModels;

namespace DyingFire
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(this);
            CustomCursor.Margin = new Thickness(p.X, p.Y, 0, 0);
        }
    }
}