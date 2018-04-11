﻿using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;

namespace LogExplorer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private ObservableCollection<LogLine> LogLineCollection = new ObservableCollection<LogLine>();
        private ObservableCollection<CheckedListItem<string>> logFilters = new ObservableCollection<CheckedListItem<string>>();
        private CollectionViewSource viewSource = new CollectionViewSource();


        public MainWindow()


        {
            InitializeComponent();





            //LoadLogFile(Environment.CurrentDirectory + "\\preview.txt");

            LoadLogFile();
            foreach (string cust in LogLineCollection.Select(w => w.Company).Distinct().OrderBy(w => w))
            {
                logFilters.Add(new CheckedListItem<string> { Item = cust, IsChecked = true });
            }

            viewSource.Filter += viewSource_Filter;
            viewSource.Source = LogLineCollection;
            logGrid.ItemsSource = viewSource.View;
            lstCompany.ItemsSource = logFilters;

            DataContext = this;

        }


        private void viewSource_Filter(object sender, FilterEventArgs e)
        {
            LogLine cust = (LogLine)e.Item;

            int count = logFilters.Where(w => w.IsChecked).Count(w => w.Item == cust.Company);

            if (count == 0)
            {
                e.Accepted = false;
                return;
            }

            e.Accepted = true;
        }

        private void btnCompanyFilter_Click(object sender, RoutedEventArgs e)
        {
            popCompany.IsOpen = true;
        }

        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckedListItem<string> item in logFilters)
            {
                item.IsChecked = true;
            }
        }

        private void btnUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckedListItem<string> item in logFilters)
            {
                item.IsChecked = false;
            }
        }

        private void ApplyFilters(object sender, RoutedEventArgs e)
        {
            viewSource.View.Refresh();
        }
    

        private void LoadLogFile(String path)
        {
            LogLineCollection = new ObservableCollection<LogLine>();
            ObservableCollection<LogLine> errorLogLineCollection = new ObservableCollection<LogLine>();
            System.IO.StreamReader sr = new System.IO.StreamReader(path);
            StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + "\\last_error.txt");

            try
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    LogLine ll = new LogLine(line);
                    if (ll.NotValid == null)
                    LogLineCollection.Add(ll);
                    else
                    {
                        sw.WriteLine(line + ";" + ll.NotValid);
                    }
                }
            }


            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sr.Close();
                sw.Close();
            }

        }


        private void LoadLogFile()
        {
            string mainSeparator = ConfigurationManager.AppSettings.Get("mainSeparator");
            string propSeparator = ConfigurationManager.AppSettings.Get("propSeparator");


            LogColumnsConfigSection section = (LogColumnsConfigSection)ConfigurationManager.GetSection("LogColumns");


            LogLineCollection = new ObservableCollection<LogLine>();
            ObservableCollection<LogLine> errorLogLineCollection = new ObservableCollection<LogLine>();
            System.IO.StreamReader sr = new System.IO.StreamReader("preview.txt");

            ColumnsCollection cc = section.ColumnItems;

            
            




            StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + "\\last_error.txt");

            //try
            //{
                string logLineString = null;
                while ((logLineString = sr.ReadLine()) != null)
                {
                    Dictionary<string, string> logLineDict = new Dictionary<string, string>();

                    string[] logLineArray = logLineString.Split(mainSeparator.ToCharArray()[0]);
                    string[] columnProps;
                    foreach (ColumnElement ce in cc)
                    {
                        columnProps = ce.ParamString.Split(',');
                        string transName = columnProps[1];

                        if (propSeparator == null)
                        {
                            logLineDict.Add(ce.ColumnName, logLineArray[int.Parse(columnProps[2])]);
                        }
                        else {
                     //   string [] trName = logLineArray.Split(propSeparator.ToCharArray()[0;




                    }


                    }




                    LogLine ll = new LogLine(logLineDict);

                    if (ll.NotValid == null)
                        LogLineCollection.Add(ll);
                    else
                    {
                        sw.WriteLine(logLineString + ";" + ll.NotValid);
                    }
                }
          //  }


           // catch (Exception ex)
           // {
                //MessageBox.Show(ex.Message);
           // }
           // finally
           // {
                sr.Close();
                sw.Close();
         //   }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ReportWindow reportWindow = new ReportWindow(LogLineCollection);
            reportWindow.Show();

        }

        private void miOpenLog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = "";
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = Environment.CurrentDirectory;
                if (openFileDialog.ShowDialog() == true)
                {
                    filePath = openFileDialog.FileName;
                    LoadLogFile(filePath);
                    logGrid.ItemsSource = LogLineCollection;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
