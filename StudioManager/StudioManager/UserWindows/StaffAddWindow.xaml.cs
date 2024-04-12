using StudioManager.ViewModel;
using System.Windows;

namespace StudioManager.UserWindows
{
    /// <summary>
    /// Interaction logic for StaffAddWindow.xaml
    /// </summary>
    public partial class StaffAddWindow : Window
    {
        public StaffAddWindow(StaffVM vm)
        {
            InitializeComponent();

            DataContext = vm;
        }
    }
}
