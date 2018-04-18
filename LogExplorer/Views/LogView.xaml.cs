
using LogExplorer.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace LogExplorer.Views
{
    /// <summary>
    /// Логика взаимодействия для LogLineView.xaml
    /// </summary>
    public partial class LogLineView : UserControl
    {
        private LogVM viewModel = new LogVM();
        /// <summary>
        /// Инициализация View, привязка ViewModel LogVM
        /// </summary>
        public LogLineView()
        {
            InitializeComponent();
            DataContext = viewModel;
        }
        /// <summary>
        /// Листенер для кнопки "Открыть лог"
        /// </summary>
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
                    ObservableCollection<LogLine> logLineCollection =  viewModel.LoadLogFile(filePath);
                    logGrid.ItemsSource = logLineCollection;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
