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
    /// FNSearchWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FNSearchWindow : Window
    {
        public FNSearchWindow()
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
//                        MessageBox.Show(msgstr + " / " + sign + " / " + exp, "~");
                        CmdString = "SELECT FNumber = (fn_sign + (case when fn_exp < 0 then 'E' else 'E+' end) + cast(fn_exp as char(12))), fn_exp, f_opcode FROM fractiontbl where fn_sign = '" + sign + "'";
                        

                        // Equal Number
                        SqlCommand cmd = new SqlCommand(CmdString, con);
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable("FractionNumbers_E");
                        sda.Fill(dt);
                        dg_fn_equal.ItemsSource = dt.DefaultView;
                        txtQuery_E.Text = dt.Rows.Count.ToString();
                        txtDiff_E.Text = "0";

                        //  Small Number
                        CmdString = "SELECT A.* from (SELECT top 100 FNumber = (fn_sign + (case when fn_exp < 0 then 'E' else 'E+' end) + cast(fn_exp as char(12))), fn_exp, fn_sign as diff, f_opcode FROM fractiontbl where fn_sign < '" + sign + "') A order by A.diff desc";
                        cmd = new SqlCommand(CmdString, con);
                        sda = new SqlDataAdapter(cmd);
                        dt = new DataTable("FractionNumbers_S");
                        sda.Fill(dt);
                        DataRow[] drs = dt.Select();
                        for (int i = 0; i < dt.Rows.Count; ++i)
                        {
                            drs[i].SetField("diff", (bf - BigDecimal.Parse((string)drs[i]["diff"])).ToString());
                        }
                        txtQuery_S.Text = dt.Rows.Count.ToString();
                        txtDiff_S.Text = (string)drs[0]["diff"];
                        dg_fn_small.ItemsSource = dt.DefaultView;

                        //  Large Number
                        CmdString = "(SELECT top 100 FNumber = (fn_sign + (case when fn_exp < 0 then 'E' else 'E+' end) + cast(fn_exp as char(12))), fn_exp, fn_sign as diff, f_opcode FROM fractiontbl where fn_sign > '" + sign + "') order by fn_sign asc";
                        cmd = new SqlCommand(CmdString, con);
                        sda = new SqlDataAdapter(cmd);
                        dt = new DataTable("FractionNumbers_L");
                        sda.Fill(dt);
                        drs = dt.Select();
                        for (int i = 0; i < dt.Rows.Count; ++i)
                        {
                            drs[i].SetField("diff", (BigDecimal.Parse((string)drs[i]["diff"]) - bf).ToString());
                        }
                        txtQuery_L.Text = dt.Rows.Count.ToString();
                        txtDiff_L.Text = (string)drs[0]["diff"];
                        dg_fn_large.ItemsSource = dt.DefaultView;
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
