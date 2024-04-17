using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StudioManager.Model.Service
{
    public static class NotifyUnsavedData
    {
        public static void Show(object obj)
        {
            var window = ((System.Windows.Window)obj);
            if (MessageBox.Show
                ("Несохраненные данные будут удалены, продолжить?",
                "Внимание!",
                MessageBoxButton.
                OKCancel,
                MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                window.Close();
            }
        }
    }
}
