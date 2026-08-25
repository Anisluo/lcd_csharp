using SciChart.Charting.Visuals;
using SciChart.Examples.ExternalDependencies.Controls.ExceptionView;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

            DispatcherUnhandledException += App_DispatcherUnhandledException;
            InitializeComponent();
        }

        private void App_DispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var exceptionView = new ExceptionView(e.Exception)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };
            exceptionView.ShowDialog();

            e.Handled = true;
        }
       
    }
}
