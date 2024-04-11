using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using StudioManager.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StudioManager.ViewModel
{
    public partial class MainVM : ObservableObject
    {
        private ObservableCollection<Staff> _staffList;

        public ObservableCollection<Staff> StaffList
        {
            get 
            { 
                return _staffList; 
            }
            set
            {
                _staffList = value;
                OnPropertyChanged(nameof(StaffList));
            }
        }

        public MainVM()
        {
            LoadStaffList();
        }

        private void LoadStaffList()
        {
            using (var db = new PostgresContext())
            {
                db.Staff.Load();
                StaffList = new ObservableCollection<Staff>(db.Staff);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
