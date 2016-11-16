using Foundation;
using UIKit;
using XamarinFormsAnalyticsWrapper.iOS.Services;

namespace AnalyticsSample.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            var gaService = AnalyticsService.GetGASInstance();
            gaService.Init("UA-xxxxxx-x", 5);

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}

