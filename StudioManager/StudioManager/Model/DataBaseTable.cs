using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioManager.Model
{
    public partial class DataBaseTable : ObservableValidator
    {
        [ObservableProperty]
        int _id;
    }
}
