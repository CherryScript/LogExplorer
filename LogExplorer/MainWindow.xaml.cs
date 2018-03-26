using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

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

            List<LogLine> logLineList = LoadLogFile(Environment.CurrentDirectory + "\\preview.txt");
            logGrid.ItemsSource = logLineList;




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
