using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Win32;
using StudioManager.Model;
using StudioManager.UserWindows;

namespace StudioManager.ViewModel
{
    public partial class StaffVM : ManagerPage
    {
        [ObservableProperty]
        private List<string> staffDataList;
        private ObservableCollection<Staff> _staffList;
        private StaffAddWindow _addWindow;
        private Staff _selected;
        private Department _selectedDepartment;
        [ObservableProperty]
        private byte[] _photo;
        [ObservableProperty]
        private List<Department> _departmentList;
        private string _search;
        [ObservableProperty]
        private string _queryString;

        public Staff Selected
        {
            get
            {
                return (Staff)Validate(_selected);
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

        public string Search
        {
            get => Find(_search);
            set => SetProperty(ref _search, value);
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

        public StaffVM()
        {
            LoadList();
        }

        protected override object Validate(object obj)
        {
            Staff staff = (Staff)obj;
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

        protected override void LoadList()
        {
            using (var db = new PostgresContext())
            {
                StaffList = new ObservableCollection<Staff>(db.Staff.FromSql($"SELECT * FROM Staff").ToList());

                DepartmentList = new List<Department>(db.Departments.FromSql($"SELECT * FROM Departments").ToList());

                StaffDataList = new List<string> (db.Staff
                    .FromSql($"SELECT Employeefullname, Employeephonenumber, Employeeemail FROM Staff")
                    .Select(s => $"{s.Employeefullname}, {s.Employeephonenumber}, {s.Employeeemail}")
                    .ToList());

            }

        }

        private string Find(string line)
        {
            return line;
        }

        private async Task<int?> GetDepartmentIdFromDatabase(Department department)
        {
            using (var context = new PostgresContext())
            {
                var departmentFromDb = await context.Departments.FirstOrDefaultAsync(d => d.Id == department.Id);

                if (departmentFromDb != null)
                {
                    return departmentFromDb.Id;
                }

                return null;
            }
        }

        [RelayCommand]
        private void Add()
        {
            var result = MessageBox.Show("Использовать форму для  добавления?","Способ добавления данных",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.Yes);
            if (result == MessageBoxResult.Yes)
            {
                Photo = File.ReadAllBytes("Assets/placeholder.png");
                CanAdd = CanEdit = CanSaveDb = false;
                _addWindow = new StaffAddWindow(this);
                Selected = new Staff();
                _addWindow.Show();
            }
            else if (result == MessageBoxResult.No)
            {
                QueryString = "INSERT INTO Staff (employeefullname, employeephonenumber, employeeemail, employeeposition, employeesex) " +
                              "VALUES ('default', '1234567890', 'default@default.com', 'default', 'М');";
                SQLForm form = new SQLForm(this);
                form.Show();
            }
        }

        [RelayCommand]
        private void Remove()
        {
            QueryString = $"DELETE FROM Staff WHERE id_employee = {Selected.Id};";
            SQLForm form = new SQLForm(this);
            form.Show();
        }

        [RelayCommand] 
        private void Edit() 
        {
            var result = MessageBox.Show("Использовать форму для изменения?", "Способ удаления данных",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.Yes);
            if (result == MessageBoxResult.Yes)
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
            else if (result == MessageBoxResult.No)
            {
                QueryString = $"UPDATE Staff SET employeefullname='default' WHERE id_employee = {Selected.Id};";
                SQLForm form = new SQLForm(this);
                form.Show();
            }
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
                        var existingStaff = context.Staff.FirstOrDefault(s => s.Id == staff.Id);

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


                    context.Database.ExecuteSqlRaw(@"UPDATE Staff
                        SET Departmentname = (
                            SELECT d.Departmentname
                            FROM Departments d
                            WHERE d.Id_Department = Staff.Id_Department
                        );");
                }

                MessageBox.Show("Новый список сохранен.", "Сохранено!", MessageBoxButton.OK);
                Debug.WriteLine("Staff : Saved successfully!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Ошибка при сохранении списка сотрудников в базе данных. \n");
            }

            Refresh();
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

                Photo = File.ReadAllBytes(filePath);
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
            LoadList();
            Debug.WriteLine("Staff : Refreshed");
        }

        [RelayCommand]
        private async void Confirm()
        {
            try
            {
                using (var context = new PostgresContext())
                {
                    await context.Database.ExecuteSqlRawAsync(QueryString);
                }
                MessageBox.Show("Запрос выполнен успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Refresh();
        }
    }
}
