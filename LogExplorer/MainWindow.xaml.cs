using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Linq;

namespace LogExplorer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void LogLineViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.LogLineVM loglineVM =new ViewModel.LogLineVM();
            LogLineViewControl.DataContext = loglineVM;
        }
        //private void btnCompanyFilter_Click(object sender, RoutedEventArgs e)
        //{
        //    popCompany.IsOpen = true;
        //}



       



        //private void miOpenLog_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        string filePath = "";
        //        OpenFileDialog openFileDialog = new OpenFileDialog();
        //        openFileDialog.InitialDirectory = Environment.CurrentDirectory;
        //        if (openFileDialog.ShowDialog() == true)
        //        {
        //            filePath = openFileDialog.FileName;
        //            // LoadLogFile(filePath);
        //            logGrid.ItemsSource = LogLineCollection;
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}
    }
}
