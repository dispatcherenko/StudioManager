using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using StudioManager.Migrations;
using StudioManager.Model;
using StudioManager.UserWindows;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace StudioManager.ViewModel
{
    public partial class UsersVM : ManagerPage
    {
        private MarketWindow _window;
        private ObservableCollection<User> _usersList;
        private ObservableCollection<Game> _gamesList;
        private User _selectedUser;
        private Game _selectedGame;

        [ObservableProperty]
        private bool _isUserSelected = false;
        [ObservableProperty]
        private bool _isGameSelected = false;

        public ObservableCollection<User> UsersList
        {
            get => _usersList;
            set => SetProperty(ref _usersList, value);
        }

        public ObservableCollection<Game> GamesList
        {
            get => _gamesList;
            set => SetProperty(ref _gamesList, value);
        }

        public User SelectedUser
        {
            get => (User)Validate(_selectedUser);
            set => SetProperty(ref _selectedUser, value);
        }

        public Game SelectedGame
        {
            get
            {
                IsGameSelected = true;
                return _selectedGame;
            }
            set
            {
                if (_selectedGame != null)
                {
                    IsGameSelected = true;
                }
                else
                {
                    IsGameSelected = false;
                }
                _selectedGame = value;
                OnPropertyChanged(nameof(SelectedGame));
            }
        }

        public UsersVM()
        {
            LoadList();
        }

        protected override object Validate(object obj)
        {
            User user = (User)obj;
            if (user != null)
            {

                IsUserSelected = true;
                IsValid = !user.HasErrors;
                Debug.WriteLine("Users : IsValid", Convert.ToString(IsValid));
            }
            else
            {
                IsUserSelected = false;
            }
            return user;
        }

        protected override void LoadList()
        {
            using (var db = new PostgresContext())
            {
                UsersList = new ObservableCollection<User>();

                db.Users.Load();

                db.Users.Include(u => u.Usergames)
                 .ThenInclude(ug => ug.IdGameNavigation)
                 .Load();

                GamesList = new ObservableCollection<Game>();

                db.Games.Load();

                foreach (var user in db.Users)
                {
                    user.Games.Clear();

                    foreach (var usergame in user.Usergames)
                    {
                        user.Games.Add(usergame.IdGameNavigation);
                    }

                    UsersList.Add(user);
                }

                foreach (var game in db.Games)
                {
                    GamesList.Add(game);
                }
            }
        }

        [RelayCommand]
        private void Market()
        {
            _window = new MarketWindow(this);
            _window.Show();
        }

        [RelayCommand] private void Buy() {
            SelectedUser.Games.Add(SelectedGame);
        }

        [RelayCommand]
        private void Refresh()
        {
            LoadList();
            Debug.WriteLine("Users : Refreshed");
        }

        [RelayCommand]
        private void Save()
        {
            try
            {
                using (var context = new PostgresContext())
                {
                    foreach (var user in UsersList)
                    {
                        var existingUser = context.Users
                            .Include(u => u.Usergames)
                            .FirstOrDefault(s => s.IdUser == user.IdUser);

                        if (existingUser != null)
                        {
                            // Обновление существующего пользователя
                            context.Entry(existingUser).CurrentValues.SetValues(user);

                            // Обновление связей UserGames
                            foreach (var game in user.Games)
                            {
                                if (!existingUser.Usergames.Any(ug => ug.IdGame == game.IdGame))
                                {
                                    existingUser.Usergames.Add(new Usergame { IdUser = user.IdUser, IdGame = game.IdGame });
                                }
                            }
                        }
                        else
                        {
                            // Добавление нового пользователя
                            context.Users.Add(user);
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
    }
}
