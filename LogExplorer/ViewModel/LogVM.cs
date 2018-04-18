using System.Collections.ObjectModel;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LogExplorer.ViewModel
{
    /// <summary>
    /// ViewModel для LogView и Model LogLine
    /// </summary>
    public class LogVM : INotifyPropertyChanged
    {
        public ObservableCollection<LogLine> LogLineCollection { get; set; }
        private LogLine selectedLine;
        /// <summary>
        /// Строка из ивента
        /// </summary>
        public LogLine SelectedLine
        {
            get { return selectedLine; }
            set
            {
                selectedLine = value;
                OnPropertyChanged("SelectedLine");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Вызов ивентхэндлера
        /// </summary>
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        /// <summary>
        /// Конструктор загрузки ViewModel
        /// </summary>
        public LogVM()
        {
            LoadLogFile(Environment.CurrentDirectory + "\\preview.txt");
        }
        /// <summary>
        /// Загрузка данных в public ObservableCollection LogLineCollection из указанного файла
        /// </summary>
        public ObservableCollection<LogLine> LoadLogFile(String path)
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
            return LogLineCollection;
        }
        private MainCommand showReportWindowCommand;
        /// <summary>
        /// ICommand для кнопки "Создать отчет." Вызывает форму ReportWindow.
        /// </summary>
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
    }
    /// <summary>
    /// Реализация интерфейса ICommand.
    /// </summary>
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


