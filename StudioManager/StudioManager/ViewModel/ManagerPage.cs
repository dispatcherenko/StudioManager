using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace StudioManager.ViewModel
{
    public abstract partial class ManagerPage : ObservableObject
    {
        [ObservableProperty]
        private bool _canAdd = true;
        [ObservableProperty]
        private bool _canEdit = false;
        [ObservableProperty]
        private bool _canRemove = false;
        [ObservableProperty]
        private bool _canSaveDb = true;
        [ObservableProperty]
        private bool _isEditing = false;
        [ObservableProperty]
        private bool _isValid = false;

        protected abstract void LoadList();
        protected abstract object Validate(object obj);
    }
}
