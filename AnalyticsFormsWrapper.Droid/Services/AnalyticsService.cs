using System;
using AnalyticsFormsWrapper.Exceptions;
using AnalyticsFormsWrapper.Services;
using Android.Content;
using Android.Gms.Analytics;
using Xamarin.Forms;

[assembly: Dependency (typeof (AnalyticsFormsWrapper.Droid.Services.AnalyticsService))]
namespace AnalyticsFormsWrapper.Droid.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        const string clientId_key = "clientId";

        static GoogleAnalytics analyticsInstance;
        static Tracker analyticsTrackerInternal;
        static AnalyticsService thisRef;

        public bool OptOut {
            set {
                analyticsInstance.AppOptOut = value;
            }
            get {
                return analyticsInstance.AppOptOut;
            }
        }

        public double DispatchPeriod {
            set {
                analyticsInstance.SetLocalDispatchPeriod ((int) value);
            }
        }

        public string ClientId {
            get {
                return AnalyticsTracker.Get (clientId_key);
            }
        }

        public bool EnableAdvertisingFeatures {
            set {
                AnalyticsTracker.EnableAdvertisingIdCollection (value);
            }
        }

        private Tracker AnalyticsTracker {
            set { analyticsTrackerInternal = value; }
            get {
                if (analyticsTrackerInternal == null) {
                    throw new ServiceNotInitializedException ();
                }

                return analyticsTrackerInternal;
            }
        }

        public static AnalyticsService GetGASInstance ()
        {
            if (thisRef == null) {
                thisRef = new AnalyticsService ();
            }

            return thisRef;
        }

        public void Init (string gaTrackingId, Context AppContext = null, int dispatchPeriodInSeconds = 10)
        {
            analyticsInstance = GoogleAnalytics.GetInstance (AppContext.ApplicationContext);
            analyticsInstance.SetLocalDispatchPeriod (dispatchPeriodInSeconds);

            AnalyticsTracker = analyticsInstance.NewTracker (gaTrackingId);
            AnalyticsTracker.EnableExceptionReporting (true);
            AnalyticsTracker.EnableAdvertisingIdCollection (true);
            AnalyticsTracker.Set (clientId_key, Guid.NewGuid ().ToString ());
        }

        public void ManualDispatch ()
        {
            analyticsInstance.DispatchLocalHits ();
        }
    }
}
