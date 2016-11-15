using Android.App;
using Android.Content.PM;
using Android.OS;
using XamarinFormsAnalyticsWrapper.Droid.Services;

namespace AnalyticsSample.Droid {
    [Activity(Label = "AnalyticsSample.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity {
        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            var gaService = AnalyticsService.GetGASInstance();
            gaService.Init("UA-87598000-1", this, 5);

            LoadApplication(new App());
        }
    }
}
