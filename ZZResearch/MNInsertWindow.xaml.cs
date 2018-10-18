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
using System.Windows.Shapes;

namespace ZZResearch
{
    /// <summary>
    /// MNInsertWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MNInsertWindow : Window
    {
        public MNInsertWindow()
        {
            InitializeComponent();
        }
        private void FillDataGrid()
        {
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            text_addr.Text = text_file.Text = text_mn.Text = "";
        }
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            FillDataGrid();
        }
        private void SearchText_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                FillDataGrid();
            }
        }
    }
}
