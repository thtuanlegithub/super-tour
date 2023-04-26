﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.Ultis;
using Super_Tour.View;
using Super_Tour.Model;
using System.Windows;
using System.Threading;
using System.Data.Entity;
using System.Windows.Threading;

namespace Super_Tour.ViewModel
{
    internal class MainPackageTypeViewModel : ObservableObject
    {
        private SUPER_TOUR db = new SUPER_TOUR();
        private List<TYPE_PACKAGE> ListTypePackage;
        private ObservableCollection<TYPE_PACKAGE> _listTypePackages = new ObservableCollection<TYPE_PACKAGE>();
        private DispatcherTimer timer = new DispatcherTimer();
        public ObservableCollection<TYPE_PACKAGE> ListTypePackages
        {
            get { return _listTypePackages; }
            set
            {
                _listTypePackages = value;
                OnPropertyChanged(nameof(ListTypePackages));
            } 
        }
        // End Test
        public ICommand OpenCreatePackageTypeViewCommand { get;private set; }
        public ICommand DeletePackageInDataGridView { get;private set; }
        public  MainPackageTypeViewModel() 
        {
            OpenCreatePackageTypeViewCommand = new RelayCommand(ExecuteOpenCreatePackageTypeViewCommand);
            DeletePackageInDataGridView = new RelayCommand(ExecuteDeletePackageCommand);
            LoadAllPackage();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {

            List<TYPE_PACKAGE> myEntities = db.TYPE_PACKAGEs.ToList();
            // Kiểm tra dữ liệu có được cập nhật chưa
            if (!myEntities.SequenceEqual(ListTypePackage))
            {
                // Dữ liệu đã được cập nhật
                // Thực hiện các xử lý cập nhật dữ liệu trong ứng dụng của bạn
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ListTypePackage = myEntities;
                    _listTypePackages.Clear();
                    foreach (TYPE_PACKAGE typePackage in ListTypePackage)
                    {
                        ListTypePackages.Add(typePackage);
                    }
                });
            }
        }

        private async Task LoadAllPackage()
        {
            await LoadDataAsync();
        }
        private async Task LoadDataAsync()
        {
            await Task.Run(() =>
            {
                 ListTypePackage = db.TYPE_PACKAGEs.ToList();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _listTypePackages.Clear();
                    foreach (TYPE_PACKAGE typePackage in ListTypePackage)
                    {
                        _listTypePackages.Add(typePackage);
                    }
                    timer.Start();
                });
            });
        }
        private void getAllPackage()
        {
            List<TYPE_PACKAGE> ListTypePackage = db.TYPE_PACKAGEs.ToList();
            _listTypePackages.Clear();
            foreach (TYPE_PACKAGE typePackage in ListTypePackage)
            {
                _listTypePackages.Add(typePackage);
            }
        }    
        private void ExecuteDeletePackageCommand(object obj)
        {
            try
            {
                TYPE_PACKAGE type_package = obj as TYPE_PACKAGE;
                db.TYPE_PACKAGEs.Remove(type_package);
                db.SaveChangesAsync();
                _listTypePackages.Remove(type_package);
                MessageBox.Show("Delete Package Type successful", "Success", MessageBoxButton.OK);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"ERROR",MessageBoxButton.OK,MessageBoxImage.Exclamation);
            }
        }
        private void ExecuteOpenCreatePackageTypeViewCommand(object obj)
        {
            CreatePackageTypeView createPackageTypeView = new CreatePackageTypeView();
            createPackageTypeView.ShowDialog();
            getAllPackage();
        }
    }
}
