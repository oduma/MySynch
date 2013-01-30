﻿using System.Windows;
using MySynch.Monitor.MVVM.ViewModels;

namespace MySynch.Monitor
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            this.DataContext = new DistributorDetailsViewModel();
        }
    }
}