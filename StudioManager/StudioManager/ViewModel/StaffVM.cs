using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;
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

        public ObservableCollection<Staff> StaffList
        {
            get
            {
                return _staffList;
            }
            set
            {
                _staffList = value;
                OnPropertyChanged(nameof(_staffList));
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

        public StaffVM()
        {
            LoadStaffList();
            Photo = ConvertPathToByte("Assets/placeholder.png");
        }

        private void LoadStaffList()
        {
            using (var db = new PostgresContext())
            {
                db.Staff.Load();
                StaffList = new ObservableCollection<Staff>(db.Staff);
            }
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

        [RelayCommand]
        private void Add()
        {
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
            OnPropertyChanged(nameof(Selected));
            _addWindow.Show();
            IsEditing = true;
        }

        [RelayCommand]
        private void Save()
        {

        }

        [RelayCommand]
        private void SaveNew()
        {
            Selected.Employeephoto = Photo;

            if (IsValid)
            {
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
        }

        [RelayCommand]
        private void RadioButton(object parameter)
        {
            Debug.WriteLine(parameter);
            Sex sex = (Sex)Enum.Parse(typeof(Sex), parameter as string);
            Selected.Employeesex = sex;
        }
    }
}
