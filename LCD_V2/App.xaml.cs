using System.Windows;
using LCD_V2.Views;

namespace LCD_V2
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Eagerly initialise the template store so the XML file gets
            // seeded / loaded at app launch, not the first time the user
            // opens 模板管理.
            var _ = TemplateStore.Library;
        }
    }
}
