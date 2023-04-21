﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Super_Tour.ViewModel.PackageViewModel.PackageTypeViewModel;
namespace Super_Tour.View.PackageView.PackageTypeView
{
    /// <summary>
    /// Interaction logic for UpdatePackageTypeView.xaml
    /// </summary>
    public partial class UpdatePackageTypeView : Window
    {
        public UpdatePackageTypeView()
        {
            InitializeComponent();
            DataContext = new UpdatePackageTypeViewModel();
        }
    }
}