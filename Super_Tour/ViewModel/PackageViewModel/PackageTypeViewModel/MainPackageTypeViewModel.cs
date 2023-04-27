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
using Super_Tour.CustomControls;

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
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            await CheckDataPerSecondAsync();
        }
        private async Task CheckDataPerSecondAsync()
        {
            try
            {
                await Task.Run(async () =>
                {
                    var myEntities = await db.TYPE_PACKAGEs.ToListAsync();
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

                });
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        private async Task LoadAllPackage()
        {
            await LoadDataAsync();
        }
        private async Task LoadDataAsync()
        {
            try
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

                    });
                });
                timer.Start();
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        private void getAllPackage()
        {
            try
            {
                List<TYPE_PACKAGE> ListTypePackage = db.TYPE_PACKAGEs.ToList();
                _listTypePackages.Clear();
                foreach (TYPE_PACKAGE typePackage in ListTypePackage)
                {
                    _listTypePackages.Add(typePackage);
                }
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }    
        private async void ExecuteDeletePackageCommand(object obj)
        {
            try
            {
                TYPE_PACKAGE type_package = obj as TYPE_PACKAGE;
                timer.Stop();
                TYPE_PACKAGE type_packageFind = await db.TYPE_PACKAGEs.FindAsync(type_package.Id_Type_Package);
                if (type_packageFind != null)
                {
                    db.TYPE_PACKAGEs.Remove(type_packageFind);
                    await db.SaveChangesAsync();
                    List<TYPE_PACKAGE> ListTypePackage = db.TYPE_PACKAGEs.ToList();
                    _listTypePackages.Clear();
                    foreach (TYPE_PACKAGE typePackage in ListTypePackage)
                    {
                        _listTypePackages.Add(typePackage);
                    }
                    MyMessageBox.ShowDialog("Delete information successful.", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                    timer.Start();
                    //_listTypePackages.Remove(type_package);
                }
                else
                {
                    MyMessageBox.ShowDialog("The package type could not be found.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        private void ExecuteOpenCreatePackageTypeViewCommand(object obj)
        {
            try
            {
                CreatePackageTypeView createPackageTypeView = new CreatePackageTypeView();
                createPackageTypeView.ShowDialog();
                getAllPackage();
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
    }
}
