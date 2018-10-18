using System.Linq;
using System.Windows;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System;
using System.Windows.Input;
using System.Globalization;

namespace ZZResearch
{
    /// <summary>
    /// PRNSearchWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PRNSearchWindow : Window
    {
        public PRNSearchWindow()
        {
            InitializeComponent();
        }

        private void FillDataGrid()
        {
            string ConString = MainWindow.GetString();
            string CmdString = string.Empty;
            using (SqlConnection con = new SqlConnection(ConString))
            {
                try
                {
                    if (radio_sign.IsChecked == true)
                    {   //  숫자 검색
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
                            {
                                throw new Exception("0은 검색할 수 없습니다.");
                            }
                            else if (bf < 1)
                            {
                                throw new Exception("지수가 있는 수의 경우 1 미만은 허용되지 않습니다.");
                            }
                            else if (bf >= 10)
                            {
                                throw new Exception("지수가 있는 수의 경우 10 이상은 허용되지 않습니다.");
                            }
                            else
                            {
                                exp = int.Parse(sign.Substring(eidx + 1));
                            }
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
                        MessageBox.Show(msgstr + " / " + sign + " / " + exp, "~");
                        CmdString = "SELECT pr_num, PRNumber = (pr_sign + (case when pr_exp < 0 then 'E' else 'E+' end) + cast(pr_exp as char(12))), pr_exp FROM primetbl where pr_sign = '" + sign + "'";
                    }
                    else if (radio_number.IsChecked == true)
                    {   //  주소 검색
                        string tmpstr = searchText.Text;
                        CmdString = "SELECT pr_num, PRNumber = (pr_sign + (case when pr_exp < 0 then 'E' else 'E+' end) + cast(pr_exp as char(12))), pr_exp FROM primetbl where pr_num = " + tmpstr;
                    }
                    SqlCommand cmd = new SqlCommand(CmdString, con);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable("PrimeNumbers");
                    sda.Fill(dt);
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("결과가 없습니다.", "알림");
                    }
                    else
                    {
                        datagrid1.ItemsSource = dt.DefaultView;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "오류");
                }   //  end try-catch
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
    }
}
