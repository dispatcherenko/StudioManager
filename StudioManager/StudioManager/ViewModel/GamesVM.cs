using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using StudioManager.Model;
using StudioManager.UserWindows;
using System.IO;

namespace StudioManager.ViewModel
{
    public partial class GamesVM : ManagerPage
    {
        private Game _selectedGame;
        [ObservableProperty]
        private Game _newGame;
        private ObservableCollection<Game> _gamesList;
        [ObservableProperty]
        private byte[] _picture;
        private GameAddWindow _addWindow;
        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Now;
        [ObservableProperty]
        private Game _latest;
        [ObservableProperty]
        private Game _popular;

        public Game SelectedGame
        {
            get => (Game)Validate(_selectedGame);
            set => SetProperty(ref _selectedGame, value);
        }
        public ObservableCollection<Game> GamesList
        {
            get => _gamesList;
            set => SetProperty(ref _gamesList, value);
        }

        public GamesVM() 
        {
            LoadList();
        }

        protected override object Validate(object obj)
        {
            Game game = (Game)obj;
            if (game != null)
            {
                CanRemove = CanEdit = true;
                IsValid = !game.HasErrors;
                Debug.WriteLine("Games : IsValid", Convert.ToString(IsValid));
            }
            else
            {
                CanRemove = CanEdit = false;
            }
            return game;
        }

        protected override void LoadList()
        {
            using (var db = new PostgresContext())
            {
                GamesList = new ObservableCollection<Game>();

                db.Games.Load();

                foreach (var game in db.Games)
                {
                    GamesList.Add(game);
                }


                // Некоррелированный подзапрос
                Latest = new List<Game>(db.Games
                    .FromSqlRaw(@"
                        SELECT *
                        FROM Games
                        WHERE gamereleasedate = (
                            SELECT MAX(gamereleasedate)
                            FROM Games
                        )")
                    .ToList())[0];

                // Коррелированный подзапрос
                try
                { 
                    Popular = new List<Game>(db.Games.
                        FromSqlRaw(@"SELECT *
                                FROM Games g
                                WHERE g.id_game = (
                                    SELECT ug.id_game
                                    FROM UserGames ug
                                    GROUP BY ug.id_game
                                    ORDER BY COUNT(ug.id_user) DESC
                                    LIMIT 1
                                );")
                        .ToList())[0];
                }
                catch
                {
                    Popular = GamesList[0];
                }

            }
        }

        [RelayCommand]
        private void Add()
        {
            Picture = File.ReadAllBytes("Assets/placeholder.png");
            CanAdd = CanEdit = CanSaveDb = false;
            _addWindow = new GameAddWindow(this);
            NewGame = new Game();
            _addWindow.Show();
        }

        [RelayCommand]
        private void Remove()
        {
            GamesList.Remove(SelectedGame);
            CanAdd = true;
            CanRemove = CanEdit = false;
        }

        [RelayCommand]
        private void Edit()
        {
            CanAdd = CanEdit = CanSaveDb = false;
            _addWindow = new GameAddWindow(this);
            NewGame = SelectedGame;
            if (NewGame.Gamepicture != null)
            {
                Picture = NewGame.Gamepicture;
            }
            OnPropertyChanged(nameof(NewGame));
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
                    foreach (var game in GamesList)
                    {
                        var existingStaff = context.Games.FirstOrDefault(s => s.Id == game.Id);

                        if (existingStaff != null)
                        {
                            // Обновление существующего сотрудника
                            context.Entry(existingStaff).CurrentValues.SetValues(game);
                        }
                        else
                        {
                            // Добавление нового сотрудника
                            context.Games.Add(game);
                        }
                    }

                    context.SaveChanges();
                }

                MessageBox.Show("Новый список сохранен.", "Сохранено!", MessageBoxButton.OK);
                Debug.WriteLine("Games : Saved successfully!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Ошибка при сохранении списка продуктов в базе данных. \n");
            }
        }

        [RelayCommand]
        private async void SaveNew()
        {
            if (IsValid)
            {
                NewGame.Gamepicture = Picture;
                NewGame.Gamereleasedate = DateOnly.FromDateTime(SelectedDate);

                SelectedGame = NewGame;

                if (IsEditing)
                {
                    GamesList[GamesList.IndexOf(SelectedGame)] = SelectedGame;
                }
                else
                {
                    GamesList.Add(SelectedGame);
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

                Picture = File.ReadAllBytes(filePath);
            }
        }

        [RelayCommand]
        private void Refresh()
        {
            LoadList();
            Debug.WriteLine("Games : Refreshed");
        }
    }
}
