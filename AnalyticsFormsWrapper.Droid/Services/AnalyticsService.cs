using System;
using System.Collections.Generic;
using AnalyticsFormsWrapper.Droid.Mapper;
using AnalyticsFormsWrapper.Enums;
using AnalyticsFormsWrapper.Exceptions;
using AnalyticsFormsWrapper.Models;
using AnalyticsFormsWrapper.Services;
using Android.Content;
using Android.Gms.Analytics;
using Android.Gms.Analytics.Ecommerce;
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
        static AnalyticsProductMapper mapper;

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

            mapper = new AnalyticsProductMapper ();

            AnalyticsTracker = analyticsInstance.NewTracker (gaTrackingId);
            AnalyticsTracker.EnableExceptionReporting (true);
            AnalyticsTracker.EnableAdvertisingIdCollection (true);
            AnalyticsTracker.Set (clientId_key, Guid.NewGuid ().ToString ());
        }

        public void ManualDispatch ()
        {
            analyticsInstance.DispatchLocalHits ();
        }

        public void TrackScreen (
            string screenName,
            string campaignUrl,
            List<ProductData> products,
            ProductActions productAction,
            ActionData actionData,
            PromotionData promotion,
            CustomDimension customDimension,
            CustomMetric customMetric
        )
        {
            AnalyticsTracker.SetScreenName (screenName);

            var builder = new HitBuilders.ScreenViewBuilder();

            if (customDimension != null)
                builder.SetCustomDimension (customDimension.DimensionIndex, customDimension.DimensionValue);

            if (customMetric != null)
                builder.SetCustomMetric (customMetric.MetricIndex, customMetric.MetricValue);

            if (!string.IsNullOrWhiteSpace (campaignUrl))
                builder.SetCampaignParamsFromUrl (campaignUrl);

            if (products != null) {
                foreach (var p in products) {
                    var product = mapper.mapProduct(p);
                    switch (productAction) {
                        case ProductActions.none:
                            builder.AddImpression (product, screenName);
                            break;
                        default:
                            builder.SetProductAction (generateCheckoutProductAction (actionData, productAction));
                            builder.AddProduct (product);
                            break;
                    }
                }
            }
            AnalyticsTracker.Send (builder.Build ());
        }

        public void TrackEvent (
            string category,
            string action,
            string label,
            long value,
            CustomDimension customDimension,
            CustomMetric customMetric
        )
        {
            var builder = new HitBuilders.EventBuilder()
                .SetAction(action)
                .SetLabel(label)
                .SetValue(value);

            if (!string.IsNullOrWhiteSpace (category))
                builder.SetCategory (category);

            if (customDimension != null)
                builder.SetCustomDimension (customDimension.DimensionIndex, customDimension.DimensionValue);

            if (customMetric != null)
                builder.SetCustomMetric (customMetric.MetricIndex, customMetric.MetricValue);

            AnalyticsTracker.Send (builder.Build ());
        }

        public void TrackException (string ExceptionMessageToTrack, bool isFatalException)
        {
            AnalyticsTracker.Send (
                new HitBuilders.ExceptionBuilder ()
                .SetDescription (ExceptionMessageToTrack)
                .SetFatal (isFatalException)
                .Build ()
            );
        }

        public void TrackAppSocial (string socialNetworkName, string socialAction, string socialTarget)
        {
            AnalyticsTracker.Send (
                new HitBuilders.SocialBuilder ()
                .SetNetwork (socialNetworkName)
                .SetAction (socialAction)
                .SetTarget (socialTarget)
                .Build ()
            );
        }

        public void TrackAppTiming (
            string categroy,
            string label,
            long value,
            string variable
        )
        {
            AnalyticsTracker.Send (
                new HitBuilders.TimingBuilder ()
                .SetCategory (categroy)
                .SetLabel (label)
                .SetValue (value)
                .SetVariable (variable)
                .Build ()
            );
        }

        public void SetCurrencyCode (string currencyCode)
        {
            if (!string.IsNullOrWhiteSpace (currencyCode))
                AnalyticsTracker.Set ("&cu", currencyCode);
        }

        public void TrackUserId (string userid)
        {
            if (!string.IsNullOrWhiteSpace (userid)) {
                AnalyticsTracker.Set ("&uid", userid);
                var builder = new HitBuilders.EventBuilder();
                AnalyticsTracker.Send (builder.Build ());
            }
        }

        ProductAction generateCheckoutProductAction (ActionData actionData, ProductActions productAction)
        {
            var pAction = new ProductAction (productAction.ToString ());

            if (!string.IsNullOrEmpty (actionData?.Id) 
                && (productAction == ProductActions.purchase
                    || productAction == ProductActions.refund))
                return pAction;
            else
                pAction.SetTransactionId (actionData.Id);

            if (!string.IsNullOrEmpty (actionData?.Affiliation))
                pAction.SetTransactionAffiliation (actionData.Affiliation);

            if (actionData?.Revenue != 0)
                pAction.SetTransactionRevenue (actionData.Revenue);

            if (actionData?.Tax != 0)
                pAction.SetTransactionTax (actionData.Tax);

            if (actionData?.Shipping != 0)
                pAction.SetTransactionShipping (actionData.Shipping);

            if (!string.IsNullOrEmpty (actionData?.Coupon))
                pAction.SetTransactionCouponCode (actionData.Coupon);

            if (!string.IsNullOrEmpty (actionData?.List))
                pAction.SetProductActionList (actionData.List);

            if (actionData?.Step != 0)
                pAction.SetCheckoutStep (actionData.Step);

            if (!string.IsNullOrEmpty (actionData?.Option))
                pAction.SetCheckoutOptions (actionData.Option);

            return pAction;
        }
    }
}
