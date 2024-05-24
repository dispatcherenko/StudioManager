using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using StudioManager.Model;
using StudioManager.Model.Service;
using StudioManager.UserWindows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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
        private TasksVM _tasks;
        [ObservableProperty]
        private GamesVM _games;
        [ObservableProperty]
        private UsersVM _users;
        [ObservableProperty]
        private QueryVM _query;

        [ObservableProperty]
        private bool _noneVisible = true;
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
                Debug.WriteLine("переключение...");
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
            Tasks = new TasksVM();
            Games = new GamesVM();
            Users = new UsersVM();
            Query = new QueryVM();
        }

        public object VisibilityCheck(object item)
        {
            Debug.WriteLine(item);
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
                            DepartmentsVisible = 
                                TasksVisible = 
                                GamesVisible = 
                                NoneVisible = 
                                UsersVisible = false;
                            Debug.WriteLine("Main : Switched to Staff");
                            break;
                        case "Отделы":
                            DepartmentsVisible = true;
                            StaffVisible = 
                                TasksVisible = 
                                GamesVisible = 
                                NoneVisible = 
                                UsersVisible = false;
                            Debug.WriteLine("Main : Switched to Departments");
                            break;
                        case "Таски":
                            TasksVisible = true;
                            StaffVisible = 
                                DepartmentsVisible = 
                                GamesVisible =
                                NoneVisible = 
                                UsersVisible = false;
                            Debug.WriteLine("Main : Switched to Tasks");
                            break;
                        case "Продукты":
                            GamesVisible = true;
                            StaffVisible = 
                                DepartmentsVisible = 
                                TasksVisible = 
                                NoneVisible = 
                                UsersVisible = false;
                            Debug.WriteLine("Main : Switched to Games");
                            break;
                        case "Пользователи":
                            UsersVisible = true;
                            StaffVisible =
                                DepartmentsVisible =
                                TasksVisible =
                                NoneVisible = 
                                GamesVisible = false;
                            Debug.WriteLine("Main : Switched to Games");
                            break;
                        case "Query":
                            SQLQuery query = new SQLQuery();
                            query.Show();
                            break;
                        default:
                            NoneVisible = true;
                            StaffVisible = 
                                DepartmentsVisible = 
                                TasksVisible = 
                                GamesVisible = 
                                UsersVisible = false;
                            break;
                    }
                }
            }

                return item;
        }

        [RelayCommand]
        private void WindowClosing(object obj)
        {

        }

        [RelayCommand]
        private void MoveTo(int id)
        {
            Debug.Write("MoveTo");
            DepartmentsVisible = true;
            StaffVisible =
                TasksVisible =
                GamesVisible =
                NoneVisible =
                UsersVisible = false;
            Debug.WriteLine("Main : Switched to Departments");
            Departments.Selected = Departments.DepartmentList.FirstOrDefault(d => d.Id == id);
        }   
    }
}
