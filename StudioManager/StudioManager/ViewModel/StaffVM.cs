using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using StudioManager.Model;
using StudioManager.UserWindows;

namespace StudioManager.ViewModel
{
    public partial class StaffVM : ObservableObject
    {
        private ObservableCollection<Staff> _list;

        StaffAddWindow _addWindow;

        [ObservableProperty]
        private Staff _selected;

        public ObservableCollection<Staff> List
        {
            get
            {
                return _list;
            }
            set
            {
                _list = value;
                OnPropertyChanged(nameof(List));
            }
        }

        public NewEmployeeVM NewEmployee{ get; set; }

        public StaffVM()
        {
            LoadStaffList();
        }

        private void LoadStaffList()
        {
            using (var db = new PostgresContext())
            {
                db.Staff.Load();
                List = new ObservableCollection<Staff>(db.Staff);
            }
        }

        [RelayCommand]
        private void Add()
        {
            _addWindow = new StaffAddWindow();
            NewEmployee = new NewEmployeeVM();
            NewEmployee.NewStaff = new Staff();
            _addWindow.Show();
        }

        [RelayCommand]
        private void Remove()
        {

        }

        [RelayCommand] 
        private void Edit() 
        {
            _addWindow = new StaffAddWindow();
            NewEmployee = new NewEmployeeVM(Selected);
            _addWindow.DataContext = NewEmployee;
            _addWindow.Show();
        }

        [RelayCommand]
        private void Save()
        {

        }
    }
}
