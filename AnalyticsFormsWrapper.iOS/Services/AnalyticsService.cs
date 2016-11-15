using System;
using Google.Analytics;
using AnalyticsFormsWrapper.Services;
using Xamarin.Forms;
using AnalyticsFormsWrapper.Exceptions;

[assembly: Dependency (typeof (AnalyticsFormsWrapper.iOS.Services.AnalyticsService))]
namespace AnalyticsFormsWrapper.iOS.Services
{
 
    public class AnalyticsService : IAnalyticsService
    {
        const string allowTrackingKey = "AllowTracking";
        static AnalyticsService thisRef;
        static ITracker gaTrackerInternal;

        public bool OptOut {
            set {
                Gai.SharedInstance.OptOut = value;
            }
            get {
                return Gai.SharedInstance.OptOut;
            }
        }

        public double DispatchPeriod {
            set {
                Gai.SharedInstance.DispatchInterval = value;
            }
            get {
                return Gai.SharedInstance.DispatchInterval;
            }
        }

        public string ClientId {
            get {
                return analyticsTracker.Get (GaiConstants.ClientId);
            }
        }

        public bool EnableAdvertisingFeatures {
            set {
                analyticsTracker.SetAllowIdfaCollection (value);
            }
            get {
                return analyticsTracker.GetAllowIdfaCollection ();
            }
        }

        private ITracker analyticsTracker {
            set {
                gaTrackerInternal = value;
            }
            get {
                if (gaTrackerInternal == null) {
                    throw new ServiceNotInitializedException ();
                }

                return gaTrackerInternal;
            }
        }

        public static AnalyticsService GetGASInstance ()
        {
            if (thisRef == null) {
                thisRef = new AnalyticsService ();
            }
            return thisRef;
        }

        public void Init (string gaTrackingId, int dispatchIntervalInSeconds = 5)
        {
            Gai.SharedInstance.OptOut = false;
            Gai.SharedInstance.DispatchInterval = dispatchIntervalInSeconds;
            analyticsTracker = Gai.SharedInstance.GetTracker (gaTrackingId);
        }

        public void ManualDispatch ()
        {
            Gai.SharedInstance.Dispatch ();
        }
    }
}
