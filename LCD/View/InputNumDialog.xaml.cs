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
    /// InputNumDialog.xaml 的交互逻辑
    /// </summary>
    public partial class InputNumDialog : Window
    {
        public int num { get; set; } = 0;
        private int max;
        public InputNumDialog(int current,int max)
        {
            InitializeComponent();
            txtVal.Text = current.ToString();
            this.max = max;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            if(txtVal.Text.Length == 0)
            {
                MessageBox.Show("请输入点号");
                txtVal.Focus();
                return;
            }
            int val = 0;
            try
            {
                val = int.Parse(txtVal.Text);
            }
            catch
            {
                MessageBox.Show("请输入数字点号");
                txtVal.Focus();
                return;
            }
            if(val <= 0|| val> max)
            {
                MessageBox.Show("请输入正确点号");
                txtVal.Focus();
                return;
            }
            num = val;
            this.Close();
        }

        private void BtnCanl_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
