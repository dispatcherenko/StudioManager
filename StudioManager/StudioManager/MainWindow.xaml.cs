using StudioManager.ViewModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using StudioManager.Model;
using Microsoft.EntityFrameworkCore;

namespace StudioManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainVM mainVM = new MainVM();

            DataContext = mainVM;
        }
    }
}