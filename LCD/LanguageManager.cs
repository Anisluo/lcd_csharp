using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace LCD
{
    internal class LanguageManager : INotifyPropertyChanged
    {
        private string _language;
        private readonly ResourceManager _resourceManager;
        private static readonly Lazy<LanguageManager> _lazy = new Lazy<LanguageManager>(() => new LanguageManager());
        public static LanguageManager Instance => _lazy.Value;
        public event PropertyChangedEventHandler PropertyChanged;

        public LanguageManager()
        {
            _language = "zh";//默认中文啊
            //获取此命名空间下Resources的Lang的资源，Lang可以修改
            _resourceManager = new ResourceManager("LCD.Resources.Lang", typeof(LanguageManager).Assembly);
        }

        public string this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }
                return _resourceManager.GetString(name);
            }
        }

        public void ChangeLanguage(CultureInfo cultureInfo)
        {
            _language = cultureInfo.Name;
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("item[]"));  //字符串集合，对应资源的值
        }

        //返回当前的语言名称啊
        public string CurrentLanguage()
        {
            return _language;
        }
    }
}
