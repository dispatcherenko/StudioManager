using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
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
using System.Linq;
using Newtonsoft.Json;

namespace StudioManager.ViewModel
{
    public partial class QueryVM : ObservableObject
    {
        private string _query;
        [ObservableProperty]
        private List<DataBaseTable> _resultList;

        public string Query
        {
            get => _query;
            set
            {
                _query = value;
                UpdateOutput();
                OnPropertyChanged(nameof(Query));
            }
        }

        public QueryVM()
        {
            ResultList = new List<DataBaseTable>();
        }

        private void UpdateOutput()
        {
            try
            {
                using (var db = new PostgresContext())
                {
                    var formattableQuery = FormattableStringFactory.Create(Query);
                    var result = db.Database.SqlQuery<DataBaseTable>(formattableQuery).ToList();
                    ResultList = new List<DataBaseTable>(result);
                }
            }
            catch { }

        }
    }
}
