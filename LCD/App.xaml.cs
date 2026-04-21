using SciChart.Charting.Visuals;
using SciChart.Examples.ExternalDependencies.Controls.ExceptionView;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LCD
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {

            //注意要是该密钥错了，就完球了。
            SciChartSurface.SetRuntimeLicenseKey(@"
                                                <LicenseContract>
                                                <Customer>optin</Customer>
                                                <OrderId>ABT211116-3422-38476</OrderId>
                                                <LicenseCount>1</LicenseCount>
                                                <IsTrialLicense>false</IsTrialLicense>
                                                <SupportExpires>11/16/2022 00:00:00</SupportExpires>
                                                <ProductCode>SC-WPF-SDK-ENTERPRISE</ProductCode>
                                                <KeyCode>lwABAAEAAAB8WbUFpdrXAQEAbQBDdXN0b21lcj1
                                                vcHRpbjtPcmRlcklkPUFCVDIxMTExNi0zNDIyLTM4NDc2O1N1
                                                YnNjcmlwdGlvblZhbGlkVG89MTYtTm92LTIwMjI7UHJvZHVjdENvZGU9U0MtV1BGLVNESy
                                                1FTlRFUlBSSVNFSk3RG0kblo0WmI/gL7YNaLa3sxb4O74Dplz+e4vdG14lDKtzXY5CbdHzfi4Aau3e</KeyCode>
                                                </LicenseContract> ");

            //捕获没有处理的异常
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            //专门捕获所有线程中的异常
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException; ;
            //专门捕获Task异常
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            //设置一个值，此值指示数据绑定 TextBox 是否应显示与源的 Text 属性值一致的字符串 为false
            System.Windows.FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;
            
            InitializeComponent();
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LogUnhandledException(e.Exception.Message + "\r\n" + e.Exception.StackTrace);
            e.SetObserved();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            LogUnhandledException("Message:"+ex.Message + "\r\nStacktrace:" + ex.StackTrace+ "\r\nInnerException:"+ex.InnerException);
        }

        private void App_DispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogUnhandledException(e.Exception.Message+"\r\n"+e.Exception.StackTrace);
            //var exceptionView = new ExceptionView(e.Exception)
            //{
            //    WindowStartupLocation = WindowStartupLocation.CenterScreen,
            //};
            //exceptionView.ShowDialog();

            e.Handled = true;
        }

        private void LogUnhandledException(string msg)
        {
            StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory+"\\record.txt", false);
            sw.WriteLine(msg);
            sw.Close();
        }
    }
}
