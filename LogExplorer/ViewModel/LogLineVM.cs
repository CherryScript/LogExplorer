using System.Collections.ObjectModel;
using System.ComponentModel;

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Win32;

namespace LogExplorer.ViewModel
{




    public class LogLineVM : INotifyPropertyChanged
    {
        public ObservableCollection<LogLine> LogLineCollection { get; set; }
        public ObservableCollection<CheckedListItem<string>> logFilter { get; set; }
        public CollectionViewSource viewSource = new CollectionViewSource();


        //private LogLine selectedLine;
        //public LogLine SelectedLine
        //{
        //    get { return selectedLine; }
        //    set
        //    {
        //        selectedLine = value;
        //        OnPropertyChanged("SelectedLine");
        //    }
        //}

        public LogLineVM()
        {
            LoadLogFile(Environment.CurrentDirectory + "\\preview.txt");


            logFilter  = new ObservableCollection<CheckedListItem<string>>();

            foreach (string cust in LogLineCollection.Select(w => w.Company).Distinct().OrderBy(w => w))
            {
                logFilter.Add(new CheckedListItem<string> { Item = cust, IsChecked = true });
            }

            viewSource.Filter += viewSource_Filter;
            viewSource.Source = LogLineCollection;
            //logGrid.ItemsSource = viewSource.View;
            //lstCompany.ItemsSource = logFilters;
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


 
      


        private void viewSource_Filter(object sender, FilterEventArgs e)
        {
            LogLine cust = (LogLine)e.Item;

            int count = logFilter.Where(w => w.IsChecked).Count(w => w.Item == cust.Company);

            if (count == 0)
            {
                e.Accepted = false;
                return;
            }

            e.Accepted = true;
        }

      


        private void LoadLogFile(String path)
        {
            LogLineCollection = new ObservableCollection<LogLine>() { };
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
                        LogLineCollection.Add(ll);
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

        private MainCommand companyFilterCommand;
        public MainCommand CompanyFilterCommand
        {
            get
            {
                return companyFilterCommand ??
                  (companyFilterCommand = new MainCommand(obj =>
                  {
                    //popCompany.IsOpen = true;
                  }));
            }
        }

        private MainCommand showReportWindowCommand;
        public MainCommand ShowReportWindowCommand
        {
            get
            {
                return showReportWindowCommand ??
                  (showReportWindowCommand = new MainCommand(obj =>
                  {
                      ReportWindow reportWindow = new ReportWindow(LogLineCollection);
                      reportWindow.Show();
                  }));
            }
        }


        private MainCommand selectAllCommand;
        public MainCommand SelectAllCommand
        {
            get
            {
                return selectAllCommand ??
                  (selectAllCommand = new MainCommand(obj =>
                  {
                      foreach (CheckedListItem<string> item in logFilter)
                      {
                          item.IsChecked = true;
                      }
                  }));
            }
        }

        private MainCommand unselectAllCommand;
        public MainCommand UnselectAllCommand
        {
            get
            {
                return unselectAllCommand ??
                  (unselectAllCommand = new MainCommand(obj =>
                  {
                      foreach (CheckedListItem<string> item in logFilter)
                      {
                          item.IsChecked = false;
                      }
                  }));
            }
        }

        private MainCommand applyFiltersCommand;
        public MainCommand ApplyFiltersCommand
        {
            get
            {
                return applyFiltersCommand ??
                  (applyFiltersCommand = new MainCommand(obj =>
                  {
                      viewSource.View.Refresh();
                  }));
            }
        }


        private MainCommand openLogCommand;
        public MainCommand OpenLogCommand
        {
            get
            {
                return openLogCommand ??
                  (openLogCommand = new MainCommand(obj =>
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
                              //MainWindow.
                              //logGrid.ItemsSource = LogLineCollection;
                            
                          }


                      }
                      catch (Exception ex)
                      {
                          MessageBox.Show(ex.Message);
                      }
                  }));
            }
        }


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

    public class CheckedListItem<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isChecked;
        private T item;

        public CheckedListItem()
        { }

        public CheckedListItem(T item, bool isChecked = false)
        {
            this.item = item;
            this.isChecked = isChecked;
        }

        public T Item
        {
            get { return item; }
            set
            {
                item = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Item"));
            }
        }


        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsChecked"));
            }
        }

    }







    public class MainCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public MainCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }

}


