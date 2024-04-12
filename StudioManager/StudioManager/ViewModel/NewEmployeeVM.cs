using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using StudioManager.Model;
using StudioManager.UserWindows;

namespace StudioManager.ViewModel
{
    public partial class NewEmployeeVM : ObservableObject
    {
        [ObservableProperty]
        private Staff _newStaff;

        public NewEmployeeVM()
        { 

        }

        public NewEmployeeVM(Staff employee)
        {
            NewStaff = employee;
        }

        [RelayCommand]
        private void Save()
        {
        }

        [RelayCommand]
        private void Exit()
        {
        }
    }
}
