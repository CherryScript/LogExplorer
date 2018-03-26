using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
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

        List<LogLine> logLineList;

        public MainWindow()
        {
            InitializeComponent();

            List<LogLine> logLineList = LoadLogFile(Environment.CurrentDirectory + "\\preview.txt");
            logGrid.ItemsSource = logLineList;

            ICollectionView view = CollectionViewSource.GetDefaultView(logLineList);

            view.Filter = str => (str as LogLine).Name.ToLower().Contains(filter.Text.ToLower());
            view.GroupDescriptions.Add(new PropertyGroupDescription("Name"));
            

            logGrid.ItemsSource = view;


        }



        private void filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (logGrid.ItemsSource is ICollectionView)
            {
                (logGrid.ItemsSource as ICollectionView).Refresh();
            }
        }


        private List<LogLine> LoadLogFile(String path)
        {
            List<LogLine> logLineList = new List<LogLine> { };
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
            return logLineList;
        }
    }
}
