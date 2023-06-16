﻿using Org.BouncyCastle.Crypto.Engines;
using Student_wpf_application.ViewModels.Command;
using Super_Tour.CustomControls;
using Super_Tour.Model;
using Super_Tour.Ultis;
using Super_Tour.Ultis.Api_Address;
using Super_Tour.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Super_Tour.ViewModel
{
    internal class CreateBookingViewModel: ObservableObject
    {
        #region Declare variable
        private SUPER_TOUR db = null;
        private MainViewModel _mainViewModel = null;
        private MainBookingViewModel _mainBookingViewModel = null;
        private TRAVEL _selectedTravel = null;
        private List<TRAVEL> _listTravelOriginal = null;
        private List<TRAVEL> _listTravelSearching = null;
        private ObservableCollection<TRAVEL> _listTravels = null;
        private List<TOURIST> _tourists = null;
        private ObservableCollection<Province> _listProvinces = null;
        private ObservableCollection<District> _listDistricts = null;
        private Province _selectedProvince;
        private District _selectedDistrict;
        private CUSTOMER _loadedCustomer = null;
        private string _selectedFilter = "Tour Name";
        private bool _isDataModified = false;
        private bool _onSearching = false;
        private string _idNumber;
        private string _customerName;
        private string _phoneNumber;
        private string _selectedGender;
        private string _searchTravel;
        private string tableBooking = "UPDATE_BOOKING";
        private string tableCustomer = "UPDATE_CUSTOMER";
        #endregion

        #region Declare binding
        public ObservableCollection<TRAVEL> ListTravels
        {
            get { return _listTravels; }
            set
            {
                _listTravels = value;
                OnPropertyChanged(nameof(ListTravels)); 
            }
        }

        public string SelectedFilter
        {
            get
            {
                return _selectedFilter;
            }
            set
            {
                _selectedFilter = value;
                OnPropertyChanged(nameof(SelectedFilter));
            }
        }

        public string IdNumber
        {
            get
            {
                return _idNumber;
            }
            set
            {
                if (string.IsNullOrEmpty(value) || value.All(char.IsDigit))
                {
                    _idNumber = value;
                    OnPropertyChanged(nameof(IdNumber));
                    CheckDataModified();
                }
            }
        }

        public string PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }
            set
            {
                if (string.IsNullOrEmpty(value) || value.All(char.IsDigit))
                {
                    _phoneNumber = value;
                    OnPropertyChanged(nameof(PhoneNumber));
                    if (_phoneNumber.Length == 10)
                        CheckAutoFillInformation();
                    CheckDataModified();
                }
            }
        }

        public string CustomerName
        {
            get
            {
                return _customerName;
            }
            set
            {
                if (string.IsNullOrEmpty(value) || value.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                {
                    _customerName = value;
                    OnPropertyChanged(nameof(CustomerName));
                    CheckDataModified();
                }
            }
        }

        public string SearchTravel
        {
            get
            {
                return _searchTravel;
            }
            set
            {
                _searchTravel = value;
                OnPropertyChanged(nameof(SearchTravel));
            }
        }

        public bool IsDataModified
        {
            get 
            {
                return _isDataModified; 
            }
            set
            {
                _isDataModified = value;
                OnPropertyChanged(nameof(IsDataModified));
            }
        }

        public string SelectedGender
        {            
            get
            {
                return _selectedGender; 
            }
            set 
            {
                _selectedGender = value;
                OnPropertyChanged(nameof(SelectedGender));
                CheckDataModified();
            } 
        }

        public District SelectedDistrict
        {
            get { return _selectedDistrict; }
            set
            {
                _selectedDistrict = value;
                OnPropertyChanged(nameof(SelectedDistrict));
                CheckDataModified();
            }
        }

        public Province SelectedProvince
        {
            get
            {
                return _selectedProvince;
            }
            set
            {
                _selectedProvince = value;
                OnPropertyChanged(nameof(SelectedProvince));
                CheckDataModified();
            }
        }

        public ObservableCollection<District> ListDistricts
        {
            get
            {
                return _listDistricts;
            }
            set
            {
                _listDistricts = value;
                OnPropertyChanged(nameof(ListDistricts));
            }
        }

        public ObservableCollection<Province> ListProvinces
        {
            get
            {
                return _listProvinces;
            }
            set
            {
                _listProvinces = value;
                OnPropertyChanged(nameof(ListProvinces));
            }
        }

        public TRAVEL SelectedTravel
        {
            get 
            {
                return _selectedTravel; 
            }
            set
            {
                _selectedTravel = value;
                CheckDataModified();    
                OnPropertyChanged(nameof(SelectedTravel));
            }
        }
        #endregion

        #region Command
        public ICommand SelectedFilterCommand { get; }
        public ICommand OpenSelectTravelForBookingViewCommand { get; }
        public ICommand SelectedProvinceCommand { get; }
        public ICommand OpenAddTouristForBookingViewCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand OnSearchTextChangedCommand { get; }
        public ICommand ViewTravelDetailCommand { get; }
        #endregion

        #region Constructor
        public CreateBookingViewModel(MainViewModel mainViewModel, MainBookingViewModel mainBookingViewModel)
        {
            // Create objects
            db = SUPER_TOUR.db;
            _mainViewModel = mainViewModel;
            _mainBookingViewModel = mainBookingViewModel;
            _tourists = new List<TOURIST>();
            _listTravels = new ObservableCollection<TRAVEL>();
            _listDistricts = new ObservableCollection<District>();

            // Create command
            SelectedFilterCommand = new RelayCommand(ExecuteSelectFilter);
            SelectedProvinceCommand = new RelayCommand(ExecuteSelectedProvinceComboBox);
            OpenAddTouristForBookingViewCommand = new RelayCommand(ExecuteOpenAddTouristForBookingViewCommand);
            OnSearchTextChangedCommand = new RelayCommand(ExecuteSearchTravel);
            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            ViewTravelDetailCommand = new RelayCommand(ExecuteViewTravelDetailCommand);

            // Load UI
            LoadTravel();
            LoadProvinces();
        }
        #endregion

        #region Load window
        private async void LoadTravel()
        {
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _listTravelOriginal = db.TRAVELs.ToList();
                            ReloadData();
                        });
                    }
                    catch (Exception ex)
                    {
                        MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                    }
                });
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }

        private void ReloadData()
        {
            ListTravels.Clear();
            if (_onSearching)
            {
                switch (_selectedFilter)
                {
                    case "Tour Name":
                        SearchByName();
                        break;
                    case "Tour Place":
                        SearchByPlace();
                        break;
                }
                foreach (TRAVEL travel in _listTravelSearching)
                {
                    ListTravels.Add(travel);
                }
            }
            else
            {
                foreach (TRAVEL travel in _listTravelOriginal)
                {
                    ListTravels.Add(travel);
                }
            }
        }

        #region Province
        private void LoadProvinces()
        {
            try
            {
                List<Province> provinces = Get_Api_Address.getProvinces();
                _listProvinces = new ObservableCollection<Province>(provinces);
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }

        private void ExecuteSelectedProvinceComboBox(object obj)
        {
            try
            {
                _listDistricts.Clear();
                List<District> districts = Get_Api_Address.getDistrict(_selectedProvince);
                foreach (District district in districts)
                {
                    _listDistricts.Add(district);
                }
                _selectedDistrict = null;
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #endregion

        #region Search
        private void ExecuteSelectFilter(object obj)
        {
            SearchTravel = "";
            _onSearching = false;
            ReloadData();
        }

        private void ExecuteSearchTravel(object obj)
        {
            if (string.IsNullOrEmpty(SearchTravel))
            {
                _onSearching = false;
                ReloadData();
            }
            else
            {
                _onSearching = true;
                ReloadData();
            }
        }

        private void SearchByName()
        {
            if (_listTravelOriginal == null || _listTravelOriginal.Count == 0)
                return;
            this._listTravelSearching = _listTravelOriginal.Where(p => p.TOUR.Name_Tour.ToLower().Contains(_searchTravel.ToLower())).ToList();
        }

        private void SearchByPlace()
        {
            if (_listTravelOriginal == null || _listTravelOriginal.Count == 0)
                return;
            this._listTravelSearching = _listTravelOriginal.Where(p => p.TOUR.PlaceOfTour.ToLower().Contains(_searchTravel.ToLower())).ToList();
        }
        #endregion

        #region View travel detail
        private void ExecuteViewTravelDetailCommand(object obj)
        {
            try
            {
                if (SelectedTravel != null)
                {
                    DetailTravelView detailTravelView = new DetailTravelView();
                    detailTravelView.DataContext = new DetailTravelViewModel(SelectedTravel);
                    detailTravelView.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
        }
        #endregion

        #region Auto fill Customer
        private void CheckAutoFillInformation()
        {
            // Dò trong DB có Phone Number đó ko
            _loadedCustomer = db.CUSTOMERs.FirstOrDefault(p => p.PhoneNumber == this.PhoneNumber);
            if (_loadedCustomer != null)
            {
                //MyMessageBox.ShowDialog("These information will be auto generated from existed customer!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                CustomerName = _loadedCustomer.Name_Customer;
                SelectedGender = _loadedCustomer.Gender;

                if (_loadedCustomer.IdNumber != null) 
                    IdNumber = _loadedCustomer.IdNumber;

                if (_loadedCustomer.Id_Province != null && _loadedCustomer.Id_District != null)
                {
                    SelectedProvince = _listProvinces.Where(p => p.codename == _loadedCustomer.Id_Province).First();

                    _listDistricts.Clear();
                    List<District> districts = Get_Api_Address.getDistrict(_selectedProvince).ToList();
                    foreach (District district in districts)
                    {
                        _listDistricts.Add(district);
                    }
                    SelectedDistrict = _selectedProvince.districts.Where(p => p.codename == _loadedCustomer.Id_District).FirstOrDefault();
                }
            }
        }
        #endregion

        #region Perform add new Booking  
        private async void ExecuteSaveCommand(object obj)
        {
            try
            {
                // Update customer information 
                if (_loadedCustomer == null)
                    _loadedCustomer = new CUSTOMER();
                _loadedCustomer.PhoneNumber = PhoneNumber;
                _loadedCustomer.Name_Customer = CustomerName;
                _loadedCustomer.IdNumber = IdNumber;
                _loadedCustomer.Gender = _selectedGender;
                _loadedCustomer.Id_Province = _selectedProvince.codename;
                _loadedCustomer.Id_District = _selectedDistrict.codename;
                db.CUSTOMERs.AddOrUpdate(_loadedCustomer); 
                await db.SaveChangesAsync();

                // Save booking to BOOKING table
                BOOKING booking = new BOOKING();
                booking.Status = "Unpaid";
                booking.Id_Travel = _selectedTravel.Id_Travel;
                booking.Id_Customer_Booking = _loadedCustomer.Id_Customer;

                decimal discount = _selectedTravel.Discount; 
                booking.TotalPrice = _selectedTravel.TOUR.PriceTour * _tourists.Count * (100 - discount) / 100;
                booking.Booking_Date = DateTime.Now;
                db.BOOKINGs.Add(booking);
                await db.SaveChangesAsync();

                // Update remaining tickets in Travel 
                booking.TRAVEL.RemainingTicket -= _tourists.Count;
                db.TRAVELs.AddOrUpdate(booking.TRAVEL);
                await db.SaveChangesAsync();    

                // Save Tourists of BOOKING
                int Id_Booking = db.BOOKINGs.Max(p => p.Id_Booking);
                foreach (TOURIST tourist in _tourists)
                {
                    tourist.Id_Booking = Id_Booking;
                    db.TOURISTs.Add(tourist);
                }
                await db.SaveChangesAsync();

                // Synchronyze real-time DB
                MainBookingViewModel.TimeBooking = MainCustomerViewModel.TimeCustomer = DateTime.Now;
                UPDATE_CHECK.NotifyChange(tableBooking, MainBookingViewModel.TimeBooking);
                UPDATE_CHECK.NotifyChange(tableCustomer, MainCustomerViewModel.TimeCustomer);

                // Process UI event
                MyMessageBox.ShowDialog("Add new booking successful!", "Notification", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                _mainViewModel.removeFirstChild();
                _mainViewModel.CurrentChildView = _mainBookingViewModel;
                _mainBookingViewModel.ReloadAfterCreateBooking();
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowDialog(ex.Message, "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            }
            finally
            {
            }
        }
        #endregion

        #region Add new Tourist
        private void ExecuteOpenAddTouristForBookingViewCommand(object obj)
        {
            AddTouristView view = new AddTouristView();
            view.DataContext = new AddTouristViewModel(_tourists);
            view.ShowDialog();
        }
        #endregion

        #region Check data modified
        private void CheckDataModified()
        {
            if (
                SelectedTravel == null 
                || SelectedProvince == null 
                || SelectedDistrict == null 
                || string.IsNullOrEmpty(IdNumber)
                || string.IsNullOrEmpty(PhoneNumber) 
                || string.IsNullOrEmpty(CustomerName)
                || string.IsNullOrEmpty(_selectedGender)
                )
                IsDataModified = false;
            else
                IsDataModified = true;
        }
        #endregion
    }
}
