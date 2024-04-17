using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using StudioManager.Model;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace StudioManager.ViewModel
{
    public partial class DepartmentsVM : ObservableObject
    {
        private ObservableCollection<Department> _departmentList { get; set; }

        private Department _selected;

        [ObservableProperty]
        private bool _canAdd = true;
        [ObservableProperty]
        private bool _canEdit = false;
        [ObservableProperty]
        private bool _canRemove = false;
        [ObservableProperty]
        private bool _canSaveDb = true;

        [ObservableProperty]
        private bool _isValid = false;

        public ObservableCollection<Department> DepartmentList
        {
            get
            {
                return _departmentList;
            }
            set
            {
                _departmentList = value;
                OnPropertyChanged(nameof(DepartmentList));
            }
        }

        public Department Selected
        {
            get
            {
                return Validate(_selected);
            }
            set
            {
                _selected = value;
                Debug.WriteLine("Departments : Selected item changed");
                OnPropertyChanged(nameof(Selected));
            }
        }

        public Department Validate(Department dep)
        {
            if (dep != null)
            {
                CanRemove = CanEdit = true;
                IsValid = !dep.HasErrors;
                Debug.WriteLine("Departments : IsValid", Convert.ToString(IsValid));
            }
            else
            {
                CanRemove = CanEdit = false;
            }
            return dep;
        }

        public DepartmentsVM()
        {
            LoadDepartmentList();
        }

        private void LoadDepartmentList()
        {
            using (var db = new PostgresContext())
            {
                DepartmentList = new ObservableCollection<Department>();

                db.Staff.Load();

                foreach (var dep in db.Departments)
                {
                    DepartmentList.Add(dep);
                }

            }
        }

        [RelayCommand]
        private void Add()
        {
            DepartmentList.Add(new Department());
            Selected = DepartmentList.Last();
        }

        [RelayCommand] 
        private void Remove()
        {
            DepartmentList.Remove(Selected);
            CanAdd = true;
            CanRemove = CanEdit = false;
        }


        [RelayCommand]
        private void Save()
        {
            try
            {
                using (var context = new PostgresContext())
                {
                    context.Departments.RemoveRange(context.Departments);

                    foreach (var dep in DepartmentList)
                    {
                        context.Departments.Add(dep);
                    }

                    context.SaveChanges();
                }

                MessageBox.Show("Новый список сохранен.", "Сохранено!", MessageBoxButton.OK);
                Debug.WriteLine("Departments : Saved successfully!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        [RelayCommand]
        private void Refresh()
        {
            LoadDepartmentList();
            Debug.WriteLine("Departments : Refreshed");
        }
    }
}
