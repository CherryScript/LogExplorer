using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

            LogLine ll = new LogLine("178;Слуцкий Дмитрий Анатольевич;РНПК;192.168.13.213;c9c38e43-ca61-4b27-8943-538d4f579990;13.12.2017 20:46:50;13.12.2017 2:20:10;1;");
            System.Console.Out(ll.Name);


        }
    }
}
