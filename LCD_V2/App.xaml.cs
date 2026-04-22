using System.Windows;
using LCD_V2.Views;

namespace LCD_V2
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Eagerly initialise the template store + metric store so the
            // XML files get seeded / loaded at app launch, not the first
            // time the user opens the respective page.
            var _   = TemplateStore.Library;
            var __  = MetricStore.Library;
            var ___ = MotionStore.Library;
        }
    }
}
