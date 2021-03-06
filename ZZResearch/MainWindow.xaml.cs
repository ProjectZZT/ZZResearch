﻿using System;
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

namespace ZZResearch
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Calculator_Click(object sender, RoutedEventArgs e)
        {
            Window win2 = new CalculatorWindow();
            win2.ShowDialog();
        }

        private void SearchMN_Click(object sender, RoutedEventArgs e)
        {
            MNSearchWindow.Instance.Show();
            MNSearchWindow.Instance.Focus();
        }

        private void SearchCN_Click(object sender, RoutedEventArgs e)
        {
            CNSearchWindow.Instance.Show();
            CNSearchWindow.Instance.Focus();
        }

        private void SearchPN_Click(object sender, RoutedEventArgs e)
        {
            Window win2 = new PNSearchWindow();
            win2.ShowDialog();
        }

        private void SearchFN_Click(object sender, RoutedEventArgs e)
        {
            Window win2 = new FNSearchWindow();
            win2.ShowDialog();
        }

        private void SearchPRN_Click(object sender, RoutedEventArgs e)
        {
            Window win2 = new PRNSearchWindow();
            win2.ShowDialog();
        }

        private void InsertMN_Click(object sender, RoutedEventArgs e)
        {
            Window win2 = new MNInsertWindow();
            win2.ShowDialog();
        }

        public static string GetString()
        {
            return "Data Source=192.168.0.12; Initial Catalog=zerodb; User ID=zzuser; Password=zzt*0702;";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var response = MessageBox.Show("정말로 종료하시겠습니까?", "Exit",
                                           MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (response == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
