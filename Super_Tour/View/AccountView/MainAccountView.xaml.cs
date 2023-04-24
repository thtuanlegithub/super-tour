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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Super_Tour.ViewModel;

namespace Super_Tour.View.AccountView
{
    /// <summary>
    /// Interaction logic for MainAccountView.xaml
    /// </summary>
    public partial class MainAccountView : UserControl
    {
        public MainAccountView()
        {
            InitializeComponent();
            this.DataContext = new MainAccountViewModel();
        }
    }
}
