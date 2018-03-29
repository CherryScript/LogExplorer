using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
            System.IO.StreamReader sr = new System.IO.StreamReader(path);
            try
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    logLineList.Add(new LogLine(line));
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sr.Close();
            }
    
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Report userReport = new UserReport(new UserReportFactory());
                userReport.CreateReport(logLineList);
                userReport.Write(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Report.xml");

                //Report ipReport = new IPReport(new IPReportFactory());
                //ipReport.CreateReport(logLineList);
                //ipReport.Write(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Report.xml");

                //Report companyReport = new CompanyReport(new CompanyReportFactory());
                //companyReport.CreateReport(logLineList);
                //companyReport.Write(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Report.xml");

                //Report companyUserReport = new CompanyUserReport(new CompanyUserReportFactory());
                //companyUserReport.CreateReport(logLineList);
                //companyUserReport.Write(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Report.xml");

                //Report errorReport = new ErrorReport(new ErrorReportFactory());
                //errorReport.CreateReport(logLineList);
                //errorReport.Write(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Report.xml");

            }
            catch (Exception ex)
            {    
                MessageBox.Show(ex.Message);
            }
         

            

        }
    }
}
