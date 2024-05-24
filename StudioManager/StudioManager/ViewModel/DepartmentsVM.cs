using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using StudioManager.Model;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace StudioManager.ViewModel
{
    public partial class DepartmentsVM : ManagerPage
    {
        private ObservableCollection<Department> _departmentList { get; set; }

        private Department _selected;

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
                return (Department)Validate(_selected);
            }
            set
            {
                _selected = value;
                Debug.WriteLine("Departments : Selected item changed");
                OnPropertyChanged(nameof(Selected));
            }
        }

        public DepartmentsVM()
        {
            LoadList();
        }
        protected override object Validate(object obj)
        {
            Department department = (Department)obj;
            if (department != null)
            {
                CanRemove = CanEdit = true;
                IsValid = !department.HasErrors;
                Debug.WriteLine("Departments : IsValid", Convert.ToString(IsValid));
            }
            else
            {
                CanRemove = CanEdit = false;
            }
            return department;
        }

        protected override void LoadList()
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

                    context.Database.ExecuteSqlRaw(@"UPDATE Departments
                        SET DepartmentCount = (
                            SELECT COUNT(s.Id_Employee)
                            FROM Staff s
                            WHERE s.Id_Department = Departments.Id_Department
                        );");
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
            LoadList();
            Debug.WriteLine("Departments : Refreshed");
        }
    }
}
