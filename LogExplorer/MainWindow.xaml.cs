using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace LogExplorer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private List<LogLine> logLineList;
        public MainWindow()
        {
            InitializeComponent();

            LoadLogFile(Environment.CurrentDirectory + "\\preview.txt");
            logGrid.ItemsSource = logLineList;

            // fitter
            //ICollectionView view = CollectionViewSource.GetDefaultView(logLineList);
            //view.Filter = str => (str as LogLine).Name.ToLower().Contains(filter.Text.ToLower());
            //view.GroupDescriptions.Add(new PropertyGroupDescription("Name"));
            //logGrid.ItemsSource = view;

        }

        //private void filter_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (logGrid.ItemsSource is ICollectionView)
        //    {
        //        (logGrid.ItemsSource as ICollectionView).Refresh();
        //    }
        //}



        private void LoadLogFile(String path)
        {
            logLineList = new List<LogLine> { };
            List<LogLine> errorLogLineList = new List<LogLine> { };
            System.IO.StreamReader sr = new System.IO.StreamReader(path);
            StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + "\\last_error.txt");
            try
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    LogLine ll = new LogLine(line);
                    if (ll.NotValid == null)
                        logLineList.Add(ll);
                    else
                    {
                        sw.WriteLine(line + ";" + ll.NotValid);
                    }
                }
            }


            catch (Exception)
            {
                throw;
            }
            finally
            {
                sr.Close();
                sw.Close();
            }

        }



        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ReportWindow reportWindow = new ReportWindow(logLineList);
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
                    logGrid.ItemsSource = logLineList;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
