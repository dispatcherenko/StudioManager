using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using StudioManager.Model;
using StudioManager.Model.Service;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace StudioManager.ViewModel
{
    public partial class MainVM : ObservableObject
    {
        private object _selectedItem;

        [ObservableProperty]
        private StaffVM _staff;
        [ObservableProperty]
        private DepartmentsVM _departments;

        [ObservableProperty]
        private bool _staffVisible = false;
        [ObservableProperty]
        private bool _departmentsVisible = false;
        [ObservableProperty]
        private bool _tasksVisible = false;
        [ObservableProperty]
        private bool _gamesVisible = false;
        [ObservableProperty]
        private bool _usersVisible = false;

        public object SelectedItem
        {
            get
            {
                return VisibilityCheck(_selectedItem);
            }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public MainVM()
        {
            Staff = new StaffVM();
            Departments = new DepartmentsVM();
        }

        public object VisibilityCheck(object item)
        {
            var listBoxItem = item as ListBoxItem;

            if (listBoxItem != null && listBoxItem.Content is StackPanel stackPanel)
            {
                var textBlock = stackPanel.Children.OfType<TextBlock>().FirstOrDefault();

                if (textBlock != null)
                {
                    switch (textBlock.Text)
                    {
                        case "Персонал":
                            StaffVisible = true;
                            DepartmentsVisible = false;
                            Debug.WriteLine("Main : Switched to Staff");
                            break;
                        case "Отделы":
                            DepartmentsVisible = true;
                            StaffVisible = false;
                            Debug.WriteLine("Main : Switched to Departments");
                            break;
                            // Добавьте другие кейсы, если необходимо
                    }
                }
            }

                return item;
        }

        [RelayCommand]
        private void WindowClosing(object obj)
        {

        }
    }
}
