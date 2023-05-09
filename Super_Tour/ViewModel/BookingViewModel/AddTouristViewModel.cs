﻿using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
namespace Super_Tour.ViewModel
{
    internal class AddTouristViewModel:ObservableObject
    {
        private TOURIST _tourist;
        private ObservableCollection<TOURIST> _listTourist;
        public TOURIST Tourist
        {
            get
            {
                return _tourist;
            }
            set
            {
                _tourist = value;
                OnPropertyChanged(nameof(TOURIST));
            }
        }
        public ICommand SaveTouristCommand { get; }
        public AddTouristViewModel(ObservableCollection<TOURIST> listTourist)
        {
            this._listTourist = listTourist;
            _tourist = new TOURIST();
            _tourist.Id_Tourist = listTourist.Count+1;
        }
        public void ExecuteSaveTouristCommand(object obj)
        {
            if(string.IsNullOrEmpty(_tourist.Name_Tourist))
            {
                MyMessageBox.ShowDialog("Please fill all information.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                return;
            }
            _listTourist.Add(_tourist);
            AddTouristView addTouristView = null;
            foreach (Window window in Application.Current.Windows)
            {
                Console.WriteLine(window.ToString());
                if (window is AddTouristView)
                {
                    addTouristView = window as AddTouristView;
                    break;
                }
            }
            addTouristView.Close();
        }
    }
}
