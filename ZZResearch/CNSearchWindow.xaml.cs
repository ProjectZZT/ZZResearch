using System;
using System.Windows;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace ZZResearch
{
    /// <summary>
    /// CNSearchWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CNSearchWindow : Window
    {
        public CNSearchWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void FillDataGrid()
        {
            string ConString = MainWindow.GetString();
            string CmdString = string.Empty;
            using (SqlConnection con = new SqlConnection(ConString))
            {
                try
                {
                    //  숫자 검색
                    //  파싱을 통해 문법 준수 여부 체크
                    string sign = searchText.Text.Trim();
                    decimal dd = decimal.Parse(sign, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent);

                    BigDecimal bf;
                    int exp = 0;
                    int eidx = sign.IndexOf('E');
                    if (eidx != -1)
                    {   //  가수 예외 처리
                        bf = BigDecimal.Parse(sign.Substring(0, eidx));
                        if (bf == 0)
                            throw new Exception("0은 검색할 수 없습니다.");
                        else if (bf < 1)
                            throw new Exception("지수가 있는 수의 경우 1 미만은 허용되지 않습니다.");
                        else if (bf >= 10)
                            throw new Exception("지수가 있는 수의 경우 10 이상은 허용되지 않습니다.");
                        else
                            exp = int.Parse(sign.Substring(eidx + 1));
                    }
                    else
                    {   //  지수와 가수 분리 작업
                        bf = BigDecimal.Parse(sign);
                        while (bf > 10 || bf < 1)
                        {
                            if (bf > 10)
                            {
                                bf /= 10;
                                ++exp;
                            }
                            else if (bf < 1)
                            {
                                bf *= 10;
                                --exp;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    sign = bf.ToString();
                    string msgstr = sign + (exp > 0 ? "E+" : "E") + exp;
                    CmdString = "SELECT CNumber = (cn_sign + (case when cn_exp < 0 then 'E' else 'E+' end) + cast(cn_exp as char(12))), child_logic_desc, child_create_date, child_mn_sign as MNumber, child_mn_exp, child_m_addr_a FROM childtbl where cn_sign = '" + sign + "'";

                    // Equal Number
                    SqlCommand cmd = new SqlCommand(CmdString, con);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable("ChildNumbers_E");
                    sda.Fill(dt);
                    dg_cn_equal.ItemsSource = dt.DefaultView;

                    //  Small Number
                    CmdString = "SELECT top 20 CNumber = (cn_sign + (case when cn_exp < 0 then 'E' else 'E+' end) + cast(cn_exp as char(12))), cn_exp, cn_sign as diff, child_logic_desc, child_create_date, (child_mn_sign + 'E+' + cast(child_mn_exp as char(12))) as MNumber, child_m_addr_a FROM childtbl where cn_sign < '" + sign + "'";
                    cmd = new SqlCommand(CmdString, con);
                    sda = new SqlDataAdapter(cmd);
                    dt = new DataTable("ChildNumbers_S");
                    sda.Fill(dt);
                    dg_cn_small.ItemsSource = dt.DefaultView;

                    //  Large Number
                    CmdString = "SELECT top 20 CNumber = (cn_sign + (case when cn_exp < 0 then 'E' else 'E+' end) + cast(cn_exp as char(12))), cn_exp, cn_sign as diff, child_logic_desc, child_create_date, (child_mn_sign + 'E+' + cast(child_mn_exp as char(12))) as MNumber, child_m_addr_a FROM childtbl where cn_sign > '" + sign + "'";
                    cmd = new SqlCommand(CmdString, con);
                    sda = new SqlDataAdapter(cmd);
                    dt = new DataTable("ChildNumbers_L");
                    sda.Fill(dt);
                    dg_cn_Large.ItemsSource = dt.DefaultView;
                }
                catch (Exception)
                {
                    MessageBox.Show("입력한 숫자를 확인해주세요.", "오류");
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            searchText.Text = "";
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
        
        private void dg_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DataRowView drv = (DataRowView)((DataGrid)sender).SelectedItem;
            string tmpaddr = "http://192.168.0.12:8000/zerozone/images/math/" + drv["child_m_addr_a"].ToString() + ".gif";
            BitmapImage bi = new BitmapImage(new Uri(tmpaddr));
            img_src.Source = bi;
        }
    }
}
