using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using StudioManager.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace StudioManager.ViewModel
{
    public partial class UsersVM : ManagerPage
    {
        private ObservableCollection<User> _usersList;
        private User _selectedUser;

        public ObservableCollection<User> UsersList
        {
            get => _usersList;
            set => SetProperty(ref _usersList, value);
        }

        public User SelectedUser
        {
            get => (User)Validate(_selectedUser);
            set => SetProperty(ref _selectedUser, value);
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
                CanRemove = CanEdit = true;
                IsValid = !user.HasErrors;
                Debug.WriteLine("Users : IsValid", Convert.ToString(IsValid));
            }
            else
            {
                CanRemove = CanEdit = false;
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

                foreach (var user in db.Users)
                {
                    user.Games.Clear();

                    foreach (var usergame in user.Usergames)
                    {
                        user.Games.Add(usergame.IdGameNavigation);
                    }

                    UsersList.Add(user);
                }
            }
        }

        [RelayCommand]
        private void Refresh()
        {
            LoadList();
            Debug.WriteLine("Users : Refreshed");
        }
    }
}
