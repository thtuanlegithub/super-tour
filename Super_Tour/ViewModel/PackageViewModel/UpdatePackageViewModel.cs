﻿using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.Ultis.Api_Address;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Drawing;
using System.Windows.Media.Imaging;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.View;
using System.Windows;
using Firebase.Storage;
using System.Data.Entity.Migrations;
using Super_Tour.View.PackageView;

namespace Super_Tour.ViewModel
{
    internal class UpdatePackageViewModel:ObservableObject
    {
        private ObservableCollection<City> _listCity;
        private ObservableCollection<TYPE_PACKAGE> _listTypePackage;
        private ObservableCollection<District> _listDistrict;
        private City _selectedCity=null;
        private string _imagePath;
        private District _selectedDistrict;
        private TYPE_PACKAGE _selectedTypePackage; // Danh
        private List<TYPE_PACKAGE> listOriginalTYpePackage; // Danh sách listPackage
        private BitmapImage _selectedImage = null; // Ảnh mặc định
        private PACKAGE package;
        private string _namePackage;
        private bool _isNewImage=false;
        private string _description;
        private string _price;
        private bool _execute = true;
        private SUPER_TOUR db = new SUPER_TOUR();
        private FirebaseStorage firebaseStorage;
        public string Price
        {
            get { return _price; }
            set
            {
                    _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        public string NamePackage
        {
            get
            {
                return _namePackage;
            }
            set
            {
                _namePackage = value;
                OnPropertyChanged(nameof(NamePackage));
            }
        }
        public BitmapImage SelectedImage
        {
            get { return _selectedImage; }
            set
            {
                _selectedImage = value;
                OnPropertyChanged(nameof(SelectedImage));
            }
        }
        public TYPE_PACKAGE SelectedTypePackage
        {
            get
            {
                return _selectedTypePackage;
            }
            set
            {
                _selectedTypePackage = value;
                OnPropertyChanged(nameof(SelectedTypePackage));
            }
        }
        public ObservableCollection<TYPE_PACKAGE> ListTypePackage
        {
            get
            {
                return _listTypePackage;
            }
            set
            {
                _listTypePackage = value;
                OnPropertyChanged(nameof(ListTypePackage));
            }
        }
        public District SelectedDistrict
        {
            get { return _selectedDistrict; }
            set
            {
                _selectedDistrict = value;
                OnPropertyChanged(nameof(SelectedDistrict));
            }
        }
        public City SelectedCity
        {
            get { return _selectedCity; }
            set
            {
                _selectedCity = value;
                OnPropertyChanged(nameof(SelectedCity));
            }
        }
        public ObservableCollection<District> ListDistrict
        {
            get
            {
                return _listDistrict;
            }
            set
            {
                _listDistrict = value;
                OnPropertyChanged(nameof(ListDistrict));
            }
        }

        public ObservableCollection<City> ListCity
        {
            get
            {
                return _listCity;
            }
            set
            {
                _listCity = value;
                OnPropertyChanged(nameof(ListCity));
            }
        }
        public ICommand SelectedCityCommand { get; }
        public ICommand UpdatePackageCommand { get; }
        public ICommand OpenPictureCommand { get; }
        public UpdatePackageViewModel()
        { 

        }
        public UpdatePackageViewModel(PACKAGE package)
        {
            this.package = package;
            _listCity = new ObservableCollection<City>();
            _listTypePackage = new ObservableCollection<TYPE_PACKAGE>();
            _listDistrict = new ObservableCollection<District>();
            firebaseStorage = new FirebaseStorage(@"supertour-30e53.appspot.com");
            //Description = package.Description_Package;
            LoadProvinces();
            LoadPackageType();
            LoadDistrict();
            SelectedDistrict = Get_Api_Address.getDistrict(_selectedCity).Where(p => p.codename == package.Id_District).FirstOrDefault();
            SelectedTypePackage = db.TYPE_PACKAGEs.Find(package.Id_Type_Package);
            SelectedImage = getInageOnline();
            Description = package.Description_Package;
            NamePackage = package.Name_Package;
            Price = package.Price.ToString();

            SelectedCityCommand = new RelayCommand(ExecuteSelectedCityComboBox);
            OpenPictureCommand = new RelayCommand(ExecuteOpenImage);
            UpdatePackageCommand = new RelayCommand(ExecuteUpdatePackage, canExecuteUpdate);

        }
        private bool canExecuteUpdate(object obj)
        {
            return _execute;
        }
        private async void ExecuteUpdatePackage(object obj)
        {
            try
            {
                if (_selectedCity == null || _selectedDistrict == null || _selectedImage == null || _selectedTypePackage == null || string.IsNullOrEmpty(_namePackage))
                {
                    MyMessageBox.ShowDialog("Please fill all information.", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                    return;
                }
                _execute = false;
                SUPER_TOUR db = new SUPER_TOUR();;

                package.Name_Package = _namePackage;
                package.Id_Province = _selectedCity.codename;
                package.Id_District = _selectedDistrict.codename;
                package.Description_Package = _description;
                package.Price = decimal.Parse(_price);
                package.Image_Package = await UploadImg();
                db.PACKAGEs.AddOrUpdate(package);
                await db.SaveChangesAsync();
                MyMessageBox.ShowDialog("Update package successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                UpdatePackageView createPackageView = null;
                foreach (Window window in Application.Current.Windows)
                {
                    Console.WriteLine(window.ToString());
                    if (window is UpdatePackageView)
                    {
                        createPackageView = window as UpdatePackageView;
                        break;
                    }
                }
                createPackageView.Close();
            }
            catch (Exception ex)
            {
                //db.PACKAGEs.Remove(_package);
                Console.WriteLine("Lỗi: " + ex.InnerException.Message);
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);

            }
            finally
            {
                _execute = true;
            }
        }
        private BitmapImage getInageOnline()
        {
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                using (WebClient webClient = new WebClient())
                {
                    byte[] data = webClient.DownloadData(package.Image_Package);
                    using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(data))
                    {
                        // Đọc ảnh từ MemoryStream và gán vào đối tượng BitmapImage
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();
                    }
                }
                return bitmapImage;
            }
            catch (Exception ex)
            {
                
                return null;
            }
        }
        private void ExecuteOpenImage(object obj)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif) | *.jpg; *.jpeg; *.png; *.gif";
            dialog.Title = "Chọn ảnh";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _imagePath = dialog.FileName;
                // Đọc hình ảnh từ tệp được chọn và lưu vào SelectedImage
                BitmapImage image = new BitmapImage(new Uri(dialog.FileName));
                SelectedImage = image;
                _isNewImage = true;
            }
        }
        private void ExecuteSelectedCityComboBox(object obj)
        {
            LoadDistrict();
        }
        private void LoadDistrict()
        {
            try
            {
                _listDistrict.Clear();
                List<District> districts = Get_Api_Address.getDistrict(_selectedCity).OrderBy(p => p.name).ToList();
                foreach (District district in districts)
                {
                    _listDistrict.Add(district);
                }
                _selectedDistrict = null;
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        private void LoadPackageType()
        {
            try
            {
                listOriginalTYpePackage = db.TYPE_PACKAGEs.ToList();
                // _listTypePackage.Clear();
                foreach (TYPE_PACKAGE type in listOriginalTYpePackage)
                {
                    _listTypePackage.Add(type);
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }

        }
        private City FindCity(string codename)
        {
            return Get_Api_Address.getCities().Where(p => p.codename == codename).FirstOrDefault();
        }
        private void LoadProvinces()
        {
            try
            {

                List<City> cities = Get_Api_Address.getCities();
                _selectedCity = cities.Where(p=>p.codename==package.Id_Province).First();
                cities = cities.OrderBy(p => p.name).ToList();
                foreach (City city in cities)
                {
                    _listCity.Add(city);
                }

            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        private async Task<string> UploadImg()
        {
            if (_isNewImage)
            {
                await firebaseStorage.Child("Images").Child("Package" + package.Id_Package.ToString()).DeleteAsync();
                Stream img = new FileStream(_imagePath, FileMode.Open, FileAccess.Read);
                var image = await firebaseStorage.Child("Images").Child("Package" + package.Id_Package.ToString()).PutAsync(img);
                return image;
            }
            return package.Image_Package;
        }
    }
}
