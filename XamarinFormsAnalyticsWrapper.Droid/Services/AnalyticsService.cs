using System;
using System.Collections.Generic;
using AnalyticsFormsWrapper.Droid.Mapper;
using XamarinFormsAnalyticsWrapper.Enums;
using XamarinFormsAnalyticsWrapper.Exceptions;
using XamarinFormsAnalyticsWrapper.Models;
using XamarinFormsAnalyticsWrapper.Services;
using Android.Content;
using Android.Gms.Analytics;
using Android.Gms.Analytics.Ecommerce;
using Xamarin.Forms;

[assembly: Dependency (typeof (XamarinFormsAnalyticsWrapper.Droid.Services.AnalyticsService))]
namespace XamarinFormsAnalyticsWrapper.Droid.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        const string clientId_key = "clientId";

        static AnalyticsService thisRef;
        static Tracker analyticsTrackerInternal;
        static GoogleAnalytics analyticsInstance;
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
                return analyticsTracker.Get (clientId_key);
            }
        }

        public bool EnableAdvertisingFeatures {
            set {
                analyticsTracker.EnableAdvertisingIdCollection (value);
            }
        }

        private Tracker analyticsTracker {
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

            analyticsTracker = analyticsInstance.NewTracker (gaTrackingId);
            analyticsTracker.EnableExceptionReporting (true);
            analyticsTracker.EnableAdvertisingIdCollection (true);
            analyticsTracker.Set (clientId_key, Guid.NewGuid ().ToString ());
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
            analyticsTracker.SetScreenName (screenName);

            var builder = new HitBuilders.ScreenViewBuilder();

            if (customDimension != null)
                builder.SetCustomDimension (customDimension.DimensionIndex, customDimension.DimensionValue);

            if (customMetric != null)
                builder.SetCustomMetric (customMetric.MetricIndex, customMetric.MetricValue);

            if (!string.IsNullOrWhiteSpace (campaignUrl))
                builder.SetCampaignParamsFromUrl (campaignUrl);

            if (promotion != null)
                builder.AddPromotion (generatePromotion (promotion));

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
            analyticsTracker.Send (builder.Build ());
        }

        public void TrackEvent (
            EventData eventData,
            CustomDimension customDimension,
            CustomMetric customMetric
        )
        {
            var builder = new HitBuilders.EventBuilder()
                                         .SetCategory(eventData.EventCategory)
                                         .SetAction(eventData.EventAction);

            if (!string.IsNullOrWhiteSpace (eventData?.EventLabel))
                builder.SetLabel (eventData.EventLabel);

            if (!string.IsNullOrWhiteSpace (eventData?.EventLabel))
                builder.SetValue (eventData.Id);

            if (customDimension != null)
                builder.SetCustomDimension (customDimension.DimensionIndex, customDimension.DimensionValue);

            if (customMetric != null)
                builder.SetCustomMetric (customMetric.MetricIndex, customMetric.MetricValue);

            analyticsTracker.Send (builder.Build ());
        }

        public void TrackException (
            string ExceptionMessage,
            bool isFatalException
        )
        {
            analyticsTracker.Send (
                new HitBuilders.ExceptionBuilder ()
                .SetDescription (ExceptionMessage)
                .SetFatal (isFatalException)
                .Build ()
            );
        }

        public void TrackSocial (
            string socialNetworkName,
            string socialAction,
            string socialTarget
        )
        {
            analyticsTracker.Send (
                new HitBuilders.SocialBuilder ()
                .SetNetwork (socialNetworkName)
                .SetAction (socialAction)
                .SetTarget (socialTarget)
                .Build ()
            );
        }

        public void TrackTiming (
            string categroy,
            string label,
            long value,
            string variable
        )
        {
            analyticsTracker.Send (
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
                analyticsTracker.Set ("&cu", currencyCode);
        }

        public void TrackUserId (string userid)
        {
            if (!string.IsNullOrWhiteSpace (userid)) {
                analyticsTracker.Set ("&uid", userid);
                var builder = new HitBuilders.EventBuilder();
                analyticsTracker.Send (builder.Build ());
            }
        }

        Promotion generatePromotion (PromotionData prom)
        {
            var promotion = new Promotion();

            if (!string.IsNullOrEmpty (prom?.Id))
                promotion.SetId (prom.Id);

            if (!string.IsNullOrEmpty (prom?.Name))
                promotion.SetName (prom.Name);

            if (!string.IsNullOrEmpty (prom?.Creative))
                promotion.SetCreative (prom.Creative);

            if (!string.IsNullOrEmpty (prom?.Position))
                promotion.SetPosition (prom.Position);

            return promotion;
        }

        ProductAction generateCheckoutProductAction (ActionData actionData, ProductActions productAction)
        {
            var pAction = new ProductAction (productAction.ToString ());

            if (actionData != null) {
                if (string.IsNullOrEmpty (actionData?.Id)
                    && (productAction == ProductActions.purchase
                        || productAction == ProductActions.refund)) {
                    return pAction;
                } else if (!string.IsNullOrEmpty (actionData?.Id)) {
                    pAction.SetTransactionId (actionData.Id);
                }

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
            }

            return pAction;
        }
    }
}
