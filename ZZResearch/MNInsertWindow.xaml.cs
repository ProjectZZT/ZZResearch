using System;
using System.Collections.Generic;
using System.IO;
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
using System.Globalization;
using System.Net;
using System.Collections.Specialized;


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
        private void SearchImageFile(object sender, RoutedEventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.Filter = "gif files (*.gif)|*.gif";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    text_file.Text = filePath;
                    string fileName = openFileDialog.SafeFileName;
                    text_addr.Text = fileName.Substring(0, fileName.IndexOf("."));
                }
                
                BitmapImage bi = new BitmapImage(new Uri(filePath));
                img_src.Source = bi;
            }


        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            text_addr.Text = text_file.Text = text_mn.Text = string.Empty;
        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //  사전처리 - 파싱을 통해 문법 준수 여부 체크
                string sign = text_mn.Text.Trim();
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

                //  입력 작업 시작
                using (WebClient Client = new System.Net.WebClient())
                {
                    //  이미지 업로드
                    Client.Headers.Add("Content-Type", "binary/octet-stream");

                    if (text_file.Text != string.Empty)
                    {
                        byte[] result = Client.UploadFile("http://192.168.0.12:8000/zerozone_test/imageupload.php", "POST", text_file.Text);

                        string phpmsg = System.Text.Encoding.UTF8.GetString(result, 0, result.Length);
                    }
                    else
                    {
                        MessageBox.Show("이미지 파일의 경로를 확인해주세요.", "오류");
                    }
                    
                    //  Child Number 생성
                    string urlAddress = "http://192.168.0.12:8000/zerozone_test/zz_calculator.gennum.php";
                    string num, owner, maa, mab, mac, desc, type;
                    num = text_mn.Text;
                    owner = "YANG";
                    maa = text_addr.Text;
                    mab = text_addr.Text + ".gif";
                    mac = string.Empty;
                    desc = string.Empty;
                    type = "base";

                    NameValueCollection postData = new NameValueCollection()
                    {
                        { "num", num },  //order: {"parameter name", "parameter value"}
                        { "owner", owner },
                        { "maa", maa },
                        { "mab", mab },
                        { "mac", mac },
                        { "desc", desc },
                        { "type", type }
                    };
                    Client.Headers.Clear();
                    string pagesource = Encoding.UTF8.GetString(Client.UploadValues(urlAddress, postData));
                    MessageBox.Show(pagesource, "OK");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "오류");
            }   //  end try-catch
        }

        private void SearchText_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                return;
            }
        }
    }
}
