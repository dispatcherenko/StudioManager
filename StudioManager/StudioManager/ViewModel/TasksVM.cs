using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using StudioManager.Model;
using StudioManager.UserWindows;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Threading.Tasks;

namespace StudioManager.ViewModel
{
    public partial class TasksVM : ManagerPage
    {
        private ObservableCollection<Model.Task> _tasksList;
        private ObservableCollection<Model.Task> _filteredList;
        private Model.Task _selectedTask;

        private TaskAddWindow _addWindow;

        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Now;

        [ObservableProperty]
        private List<Department> _departmentList;
        [ObservableProperty]
        private Department _selectedDepartment;
        private Department _filteredDepartment;

        [ObservableProperty]
        private List<Game> _gamesList;
        [ObservableProperty]
        private Game _selectedGame;
        private Game _filteredGame;

        [ObservableProperty]
        private List<string> _statesList;
        [ObservableProperty]
        private string _selectedState = "Sent";
        private string _filteredState;

        [ObservableProperty]
        private List<string> _groupsList = new List<string>();
        private string _filteredGroup;

        public ObservableCollection<Model.Task> TasksList
        {
            get
            {
                return _tasksList;
            }
            set
            {
                _tasksList = value;
                OnPropertyChanged(nameof(TasksList));
            }
        }

        public ObservableCollection<Model.Task> FilteredList
        {
            get
            {
                return _filteredList;
            }
            set
            {
                _filteredList = value;
                OnPropertyChanged(nameof(FilteredList));
            }
        }

        public Model.Task SelectedTask
        {
            get
            {
                return (Model.Task)Validate(_selectedTask);
            }
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
            }
        }

        public Department FilteredDepartment
        {
            get
            {
                return _filteredDepartment;
            }
            set
            {
                _filteredDepartment = value;
                Filter(TasksList);
                OnPropertyChanged(nameof(FilteredDepartment));
            }
        }

        public Game FilteredGame
        {
            get
            {
                return _filteredGame;
            }
            set
            {
                _filteredGame = value;
                Filter(TasksList);
                OnPropertyChanged(nameof(_filteredGame));
            }
        }

        public string FilteredGroup
        {
            get
            {
                return _filteredGroup;
            }
            set
            {
                _filteredGroup = value;
                Filter(TasksList);
                OnPropertyChanged(nameof(_filteredGroup));
            }
        }

        public string FilteredState
        {
            get
            {
                return _filteredState;
            }
            set
            {
                _filteredState = value;
                Filter(TasksList);
                OnPropertyChanged(nameof(_filteredState));
            }
        }

        public TasksVM()
        {
            LoadList();
        }

        protected override object Validate(object obj)
        {
            Model.Task task = (Model.Task)obj;
            if (task != null)
            {
                CanRemove = CanEdit = true;
                IsValid = !task.HasErrors;
                Debug.WriteLine("Task : IsValid", Convert.ToString(IsValid));
            }
            else
            {
                CanRemove = CanEdit = false;
            }
            return task;
        }

        protected override void LoadList()
        {
            using (var db = new PostgresContext())
            {
                TasksList = new ObservableCollection<Model.Task>();
                StatesList = new List<string>();
                GroupsList = new List<string>();

                StatesList.Add("Все");
                GroupsList.Add("Все");

                db.Staff.Load();

                foreach (var task in db.Tasks)
                {
                    TasksList.Add(task);
                    if (!(StatesList.Contains(task.Taskstate)))
                    {
                        StatesList.Add(task.Taskstate);
                    }
                    if (!(GroupsList.Contains(task.Taskgroup)))
                    {
                        GroupsList.Add(task.Taskgroup);
                    }
                }

                DepartmentList = new List<Department>();

                DepartmentList.Add(new Department { Departmentname = "Все" });

                db.Departments.Load();

                foreach (var dep in db.Departments)
                {
                    DepartmentList.Add(dep);
                }

                GamesList = new List<Game>();

                GamesList.Add(new Game { Gamename = "Все" });

                db.Games.Load();

                foreach (var game in db.Games)
                {
                    GamesList.Add(game);
                }
            }

            Filter(TasksList);
        }

        [RelayCommand]
        private void Add()
        {
            CanAdd = CanEdit = CanSaveDb = false;
            _addWindow = new TaskAddWindow(this);
            SelectedTask = new Model.Task();
            _addWindow.Show();
        }

        [RelayCommand]
        private void Remove()
        {
            TasksList.Remove(SelectedTask);
            CanAdd = true;
            CanRemove = CanEdit = false;
        }

        [RelayCommand]
        private void Edit()
        {
            CanAdd = CanEdit = CanSaveDb = false;
            _addWindow = new TaskAddWindow(this);
            OnPropertyChanged(nameof(SelectedTask));
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
                    foreach (var task in TasksList)
                    {
                        var existingTask = context.Tasks.FirstOrDefault(s => s.IdTask == task.IdTask);

                        if (existingTask != null)
                        {
                            // Обновление существующего сотрудника
                            context.Entry(existingTask).CurrentValues.SetValues(task);
                        }
                        else
                        {
                            // Добавление нового сотрудника
                            context.Tasks.Add(task);
                        }
                    }

                    context.SaveChanges();
                }

                MessageBox.Show("Новый список сохранен.", "Сохранено!", MessageBoxButton.OK);
                Debug.WriteLine("Task : Saved successfully!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Ошибка при сохранении списка тасков в базе данных. \n");
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

        private async Task<int?> GetGameIdFromDatabase(Game game)
        {
            using (var context = new PostgresContext())
            {
                var gameFromDb = await context.Games.FirstOrDefaultAsync(d => d.IdGame == game.IdGame);

                if (gameFromDb != null)
                {
                    return gameFromDb.IdGame;
                }

                return null;
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
                        SelectedTask.IdDepartment = idDepartmentFromDatabase;
                    }
                    else
                    {
                        MessageBox.Show("Отдел не найден в базе данных.");
                        return;
                    }
                }

                if (SelectedGame != null)
                {
                    int? idGameIdFromDatabase = await GetGameIdFromDatabase(SelectedGame);

                    if (idGameIdFromDatabase.HasValue)
                    {
                        SelectedTask.IdGame = idGameIdFromDatabase;
                    }
                    else
                    {
                        MessageBox.Show("Продукт не найден в базе данных.");
                        return;
                    }
                }

                SelectedTask.Taskstate = SelectedState;

                SelectedTask.Taskdeadline = DateOnly.FromDateTime(SelectedDate);

                if (IsEditing)
                {
                    TasksList[TasksList.IndexOf(SelectedTask)] = SelectedTask;
                }
                else
                {
                    TasksList.Add(SelectedTask);
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
        private void Refresh()
        {
            LoadList();
            Debug.WriteLine("Tasks : Refreshed");
        }

        [RelayCommand]
        private void Filter(ObservableCollection<Model.Task> list) 
        {
            FilteredList = new ObservableCollection<Model.Task> (list);

            if (FilteredDepartment != null && FilteredDepartment.Departmentname != "Все")
            {
                FilteredList = new ObservableCollection<Model.Task>(FilteredList.Where(t => t.IdDepartment == FilteredDepartment.IdDepartment));
            }

            if (FilteredGame != null && FilteredGame.Gamename != "Все")
            {
                FilteredList = new ObservableCollection<Model.Task>(FilteredList.Where(t => t.IdGame == FilteredGame.IdGame));
            }
            Debug.WriteLine("Tasks : Filtered");
        }
    }
}
