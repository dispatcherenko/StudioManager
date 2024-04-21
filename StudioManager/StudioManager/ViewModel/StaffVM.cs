using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using StudioManager.Model;
using StudioManager.UserWindows;

namespace StudioManager.ViewModel
{
    public partial class StaffVM : ObservableObject
    {
        private ObservableCollection<Staff> _staffList;

        private StaffAddWindow _addWindow;

        private Staff _selected;

        private Department _selectedDepartment;

        public Staff Selected
        {
            get
            {
                return Validate(_selected);
            }
            set
            {
                _selected = value;
                Debug.WriteLine("Staff : Selected item changed");
                OnPropertyChanged(nameof(Selected));
            }
        }

        public Department SelectedDepartment
        {
            get 
            { 
                return _selectedDepartment; 
            }
            set 
            { 
                _selectedDepartment = value;
                Debug.WriteLine("Staff : Department changed");
                OnPropertyChanged(nameof(SelectedDepartment)); 
            } 
        }

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

        [ObservableProperty]
        private byte[] _photo;

        [ObservableProperty]
        private bool _canAdd = true;
        [ObservableProperty]
        private bool _canEdit  = false;
        [ObservableProperty]
        private bool _canRemove  = false;
        [ObservableProperty]
        private bool _canSaveDb  = true;

        [ObservableProperty]
        private bool _isEditing = false;
        [ObservableProperty]
        private bool _isValid = false;

        [ObservableProperty]
        private List<Department> _departmentList;

        public StaffVM()
        {
            LoadStaffList();
        }

        public Staff Validate(Staff staff)
        {
            if (staff != null)
            {
                CanRemove = CanEdit = true;
                IsValid = !staff.HasErrors;
                Debug.WriteLine("Staff : IsValid", Convert.ToString(IsValid));
            }
            else
            {
                CanRemove = CanEdit = false;
            }
            return staff;
        }

        public byte[] ConvertPathToByte(string path)
        {
            return File.ReadAllBytes(path);
        }


        private void LoadStaffList()
        {
            using (var db = new PostgresContext())
            {
                StaffList = new ObservableCollection<Staff>();

                db.Staff.Load();

                foreach(var staff in db.Staff)
                {
                    StaffList.Add(staff);
                }

                DepartmentList = new List<Department>();

                db.Departments.Load();

                foreach (var dep in db.Departments)
                {
                    DepartmentList.Add(dep);
                }
            }
        }

        private async Task<int?> GetDepartmentIdFromDatabase(Department department)
        {
            using (var context = new PostgresContext())
            {
                var departmentFromDb = await context.Departments.FirstOrDefaultAsync(d => d.IdDepartment == department.IdDepartment);

                if (departmentFromDb != null)
                {
                    return departmentFromDb.IdDepartment;
                }

                return null;
            }
        }

        [RelayCommand]
        private void Add()
        {
            Photo = ConvertPathToByte("Assets/placeholder.png");
            CanAdd = CanEdit = CanSaveDb = false;
            _addWindow = new StaffAddWindow(this);
            Selected = new Staff();
            _addWindow.Show();
        }

        [RelayCommand]
        private void Remove()
        {
            StaffList.Remove(Selected);
            CanAdd = true;
            CanRemove = CanEdit = false;
        }

        [RelayCommand] 
        private void Edit() 
        {
            CanAdd = CanEdit = CanSaveDb = false;
            _addWindow = new StaffAddWindow(this);
            if (Selected.Employeephoto != null)
            {
                Photo = Selected.Employeephoto;
            }
            OnPropertyChanged(nameof(Selected));
            _addWindow.Show();
            IsEditing = true;
        }

        [RelayCommand]
        private void Save()
        {
            try
            {
                using (var context = new PostgresContext())
                {
                    foreach (var staff in StaffList)
                    {
                        var existingStaff = context.Staff.FirstOrDefault(s => s.IdEmployee == staff.IdEmployee);

                        if (existingStaff != null)
                        {
                            // Обновление существующего сотрудника
                            context.Entry(existingStaff).CurrentValues.SetValues(staff);
                        }
                        else
                        {
                            // Добавление нового сотрудника
                            context.Staff.Add(staff);
                        }
                    }

                    context.SaveChanges();
                }

                MessageBox.Show("Новый список сохранен.", "Сохранено!", MessageBoxButton.OK);
                Debug.WriteLine("Staff : Saved successfully!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Ошибка при сохранении списка сотрудников в базе данных. \n");
            }
        }

        [RelayCommand]
        private async void SaveNew()
        {
            if (IsValid)
            {
                if (SelectedDepartment != null)
                {
                    int? idDepartmentFromDatabase = await GetDepartmentIdFromDatabase(SelectedDepartment);

                    if (idDepartmentFromDatabase.HasValue)
                    {
                        Selected.IdDepartment = idDepartmentFromDatabase;
                    }
                    else
                    {
                        MessageBox.Show("Отдел не найден в базе данных.");
                        return;
                    }
                }

                if (IsEditing)
                {
                    StaffList[StaffList.IndexOf(Selected)] = Selected;
                }
                else
                {
                    StaffList.Add(Selected);
                }

                _addWindow.Close();
                CanAdd = CanSaveDb = true;
            }

            IsEditing = false;
        }

        [RelayCommand]
        private void Exit()
        {
            if (MessageBox.Show
                ("Несохраненные данные будут удалены, продолжить?",
                "Внимание!",
                MessageBoxButton.
                OKCancel,
                MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                _addWindow.Close();
            }
            CanAdd = CanSaveDb = true;
            IsEditing = false;
        }

        [RelayCommand]
        private void SetImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg; *.jpeg; *.png; *.gif)|*.jpg; *.jpeg; *.png; *.gif",
                Title = "Выберите изображение"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                Photo = ConvertPathToByte(filePath);
            }

            Selected.Employeephoto = Photo;
        }

        [RelayCommand]
        private void RadioButton(object parameter)
        {
            Debug.WriteLine("Staff : Sex chaned to: ");
            Selected.Employeesex = parameter as string;
        }

        [RelayCommand]
        private void Refresh()
        {
            LoadStaffList();
            Debug.WriteLine("Staff : Refreshed");
        }
    }
}
