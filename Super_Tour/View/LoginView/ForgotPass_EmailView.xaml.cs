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
using Super_Tour.ViewModel.LoginViewModel;
namespace Super_Tour.View.LoginView
{
    /// <summary>
    /// Interaction logic for ForgotPass_EmailView.xaml
    /// </summary>
    public partial class ForgotPass_EmailView : Window
    {
        public ForgotPass_EmailView()
        {
            InitializeComponent();
            DataContext = new ForgotPass_EmailViewModel();
        }
    }
}