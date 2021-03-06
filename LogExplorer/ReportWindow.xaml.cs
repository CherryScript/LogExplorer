﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace LogExplorer
{
    /// <summary>
    /// Логика взаимодействия для ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        private string ReportFolder;
        private ObservableCollection<LogLine> LogLineCollection;
        /// <summary>
        /// Конструктор инициализации MV
        /// </summary>
        public ReportWindow(ObservableCollection<LogLine> logLineCollection)
        {
            ReportFolder = Environment.CurrentDirectory;
            this.LogLineCollection = logLineCollection;
            InitializeComponent();

            tbReportFolder.Text = ReportFolder;
            dpInDate.SelectedDate = new DateTime(2017, 06, 21, 0, 0, 0);
            dpOutDate.SelectedDate = new DateTime(2017, 06, 23, 0, 0, 0);
        }
        /// <summary>
        /// Листенер нажатия на кнопку "Создать отчеты"
        /// </summary>
        private void btnCreateReport_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            bool xml = (bool)cbXML.IsChecked;
            bool xls = (bool)cbXLS.IsChecked;

            dict.Add("Company", tbCompany.Text);
            dict.Add("InDate", dpInDate.SelectedDate);
            dict.Add("OutDate", dpOutDate.SelectedDate);

            try
            {

                if (cbUserReportBox.IsChecked == true)
                {
                    Report userReport = new UserReport(new UserReportFactory());
                    userReport.CreateReport(LogLineCollection, dict);
                    userReport.Write(ReportFolder, xml, xls);
                }
                if (cbIPReportBox.IsChecked == true)
                {
                    Report ipreport = new IPReport(new IPReportFactory());
                    ipreport.CreateReport(LogLineCollection, dict);
                    ipreport.Write(ReportFolder, xml, xls);
                }
                if (cbCompanyReportBox.IsChecked == true)
                {
                    Report companyReport = new CompanyReport(new CompanyReportFactory());
                    companyReport.CreateReport(LogLineCollection, dict);
                    companyReport.Write(ReportFolder, xml, xls);
                }
                if (cbCompanyUserReportBox.IsChecked == true)
                {
                    Report companyUserReport = new CompanyUserReport(new CompanyUserReportFactory());
                    companyUserReport.CreateReport(LogLineCollection, dict);
                    companyUserReport.Write(ReportFolder, xml, xls);
                }
                if (cbErrorReportBox.IsChecked == true)
                {
                    Report errorreport = new ErrorReport(new ErrorReportFactory());
                    errorreport.CreateReport(LogLineCollection, dict);
                    errorreport.Write(ReportFolder, xml, xls);
                }
                MessageBox.Show("Отчеты сохранены в " + ReportFolder);

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }

        }
        /// <summary>
        /// Листенер нажатия на кнопку "Обзор"
        /// </summary>
        private void btnBrowse_Click_(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlgFolder = new System.Windows.Forms.FolderBrowserDialog();
            if (dlgFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ReportFolder = dlgFolder.SelectedPath;
                dlgFolder.SelectedPath = Environment.CurrentDirectory;

                tbReportFolder.Text = ReportFolder;
            }
        }
        /// <summary>
        /// Листенер нажатия на кнопку "Отмена"
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
