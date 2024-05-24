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
using System.Windows.Navigation;

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

        private string _search;

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
                Filter();
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
                Filter();
                OnPropertyChanged(nameof(FilteredGame));
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
                Filter();
                OnPropertyChanged(nameof(FilteredGroup));
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
                Filter();
                OnPropertyChanged(nameof(FilteredState));
            }
        }

        public string Search
        {
            get => _search;
            set 
            {
                _search = value;
                Debug.WriteLine("Tasks : Searching...");
                Find(_search);
                OnPropertyChanged(nameof(Search));
            }
        }


        public TasksVM()
        {
            LoadList();

            FilteredDepartment = DepartmentList[0];
            FilteredGame = GamesList[0];
            FilteredGroup = GroupsList[0];
            FilteredState = StatesList[0];  
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
                StatesList = new List<string>(["Все"]);
                GroupsList = new List<string>(["Все"]);

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

                DepartmentList = new List<Department>([new Department { Departmentname = "Все" }]);

                db.Departments.Load();

                foreach (var dep in db.Departments)
                {
                    DepartmentList.Add(dep);
                }

                GamesList = new List<Game>([new Game { Gamename = "Все" }]);

                db.Games.Load();

                foreach (var game in db.Games)
                {
                    GamesList.Add(game);
                }
            }

            Filter();
            Find("");
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
                        var existingTask = context.Tasks.FirstOrDefault(s => s.Id == task.Id);

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
                var departmentFromDb = await context.Departments.FirstOrDefaultAsync(d => d.Id == department.Id);

                if (departmentFromDb != null)
                {
                    return departmentFromDb.Id;
                }

                return null;
            }
        }

        private async Task<int?> GetGameIdFromDatabase(Game game)
        {
            using (var context = new PostgresContext())
            {
                var gameFromDb = await context.Games.FirstOrDefaultAsync(d => d.Id == game.Id);

                if (gameFromDb != null)
                {
                    return gameFromDb.Id;
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
        private void Filter() 
        {
            FilteredList = new ObservableCollection<Model.Task>(TasksList);

            if (FilteredDepartment != null && FilteredDepartment.Departmentname != "Все")
            {
                FilteredList = new ObservableCollection<Model.Task>(FilteredList.Where(t => t.IdDepartment == FilteredDepartment.Id));
            }

            if (FilteredGame != null && FilteredGame.Gamename != "Все")
            {
                FilteredList = new ObservableCollection<Model.Task>(FilteredList.Where(t => t.IdGame == FilteredGame.Id));
            }

            if (FilteredGroup != null && FilteredGroup != "Все")
            {
                FilteredList = new ObservableCollection<Model.Task>(FilteredList.Where(t => t.Taskgroup == FilteredGroup));
            }

            if (FilteredState != null && FilteredState != "Все")
            {
                FilteredList = new ObservableCollection<Model.Task>(FilteredList.Where(t => t.Taskstate == FilteredState));
            }

            Debug.WriteLine("Tasks : Filtered");
        }

        private void Find(string line)
        {
            FilteredList = new ObservableCollection<Model.Task>(TasksList);

            if (!string.IsNullOrEmpty(line))
            {
                FilteredList = new ObservableCollection<Model.Task>(FilteredList.Where(t => t.Taskname.Contains(line, StringComparison.OrdinalIgnoreCase)));
            }

            Debug.WriteLine("Tasks : Filtered");
        }
    }
}
