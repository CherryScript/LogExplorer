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

namespace LogExplorer.Views
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class LogLineView : UserControl
    {
        public LogLineView()
        {
           InitializeComponent();
        }

        private void btnCompanyFilter_Click(object sender, RoutedEventArgs e)
        {
            popCompany.IsOpen = true;
        }

         //private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        //{
        //    foreach (CheckedListItem<string> item in logFilters)
        //    {
        //        item.IsChecked = true;
        //    }
        //}

        //private void btnUnselectAll_Click(object sender, RoutedEventArgs e)
        //{
        //    foreach (CheckedListItem<string> item in logFilters)
        //    {
        //        item.IsChecked = false;
        //    }
        //}

        //private void ApplyFilters(object sender, RoutedEventArgs e)
        //{
        //    viewSource.View.Refresh();
        //}
    }
}
