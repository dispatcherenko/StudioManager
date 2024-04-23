using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using StudioManager.ViewModel;

namespace StudioManager.UserWindows
{
    /// <summary>
    /// Interaction logic for GameAddWindow.xaml
    /// </summary>
    public partial class GameAddWindow : Window
    {
        public GameAddWindow(GamesVM vm)
        {
            InitializeComponent();

            DataContext = vm;
        }
    }
}
