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

namespace LCD.View
{
    /// <summary>
    /// 登录控件
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void OnBnClickedEnsure(object sender, RoutedEventArgs e)
        {
            if (UserName.Text.ToLower()=="admin")
            {
                if (PassWord.Text.ToLower()=="admin")
                {
                    this.DialogResult = true;
                }
                else
                {
                    MessageBox.Show("密码错误");
                    this.DialogResult = false;
                }
            }
            this.Close();
        }
    }
}
