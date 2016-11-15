using System;
using Google.Analytics;
using XamarinFormsAnalyticsWrapper.Services;
using Xamarin.Forms;
using XamarinFormsAnalyticsWrapper.Exceptions;
using System.Collections.Generic;
using XamarinFormsAnalyticsWrapper.Models;
using XamarinFormsAnalyticsWrapper.Enums;
using XamarinFormsAnalyticsWrapper.iOS.Mapper;
using Foundation;

[assembly: Dependency(typeof(XamarinFormsAnalyticsWrapper.iOS.Services.AnalyticsService))]
namespace XamarinFormsAnalyticsWrapper.iOS.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        const string allowTrackingKey = "AllowTracking";

        static AnalyticsService thisRef;
        static ITracker analyticsTrackerInternal;
        static AnalyticsProductMapper mapper;

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
                return analyticsTracker.Get(GaiConstants.ClientId);
            }
        }

        public bool EnableAdvertisingFeatures {
            set {
                analyticsTracker.SetAllowIdfaCollection(value);
            }
            get {
                return analyticsTracker.GetAllowIdfaCollection();
            }
        }

        private ITracker analyticsTracker {
            set {
                analyticsTrackerInternal = value;
            }
            get {
                if (analyticsTrackerInternal == null) {
                    throw new ServiceNotInitializedException();
                }

                return analyticsTrackerInternal;
            }
        }

        public static AnalyticsService GetGASInstance()
        {
            if (thisRef == null) {
                thisRef = new AnalyticsService();
            }
            return thisRef;
        }

        public void Init(string gaTrackingId, int dispatchIntervalInSeconds = 5)
        {
            Gai.SharedInstance.OptOut = false;
            Gai.SharedInstance.DispatchInterval = dispatchIntervalInSeconds;
            analyticsTracker = Gai.SharedInstance.GetTracker(gaTrackingId);
            mapper = new AnalyticsProductMapper();
        }

        public void ManualDispatch()
        {
            Gai.SharedInstance.Dispatch();
        }

        public void TrackScreen(
            string screenName,
            string campaignUrl,
            List<ProductData> products,
            ProductActions productAction,
            ActionData actionData,
            PromotionData promotion,
            List<CustomDimension> customDimensions,
            List<CustomMetric> customMetrics
        )
        {
            analyticsTracker.Set(GaiConstants.ScreenName, screenName);

            var builder = DictionaryBuilder.CreateScreenView();

            if (customDimensions?.Count > 0)
                foreach (var customDimension in customDimensions)
                    builder.Set(customDimension.DimensionValue, Fields.CustomDimension((nuint)customDimension.DimensionIndex));

            if (customMetrics?.Count > 0)
                foreach (var customMetric in customMetrics)
                    builder.Set(Convert.ToString(customMetric.MetricValue), Fields.CustomMetric((nuint)customMetric.MetricIndex));

            if (promotion != null)
                builder.AddPromotion(generatePromotion(promotion));

            if (products != null) {
                foreach (var p in products) {
                    var product = mapper.mapProduct(p);
                    switch (productAction) {
                        case ProductActions.none:
                        builder.AddProductImpression(product, screenName, "App");
                        break;
                        default:
                        builder.SetProductAction(generateCheckoutProductAction(actionData, productAction));
                        builder.AddProduct(product);
                        break;
                    }
                }
            }
            analyticsTracker.Send(builder.Build());
        }

        public void TrackEvent(
            EventData eventData,
            List<CustomDimension> customDimensions,
            List<CustomMetric> customMetrics
        )
        {
            var builder = DictionaryBuilder.CreateEvent(
                eventData.EventCategory,
                eventData.EventAction,
                !string.IsNullOrWhiteSpace(eventData.EventLabel) ? eventData.EventLabel : string.Empty,
                new NSNumber(eventData.Id)
            );

            if (customDimensions?.Count > 0)
                foreach (var customDimension in customDimensions)
                    builder.Set(customDimension.DimensionValue, Fields.CustomDimension((nuint)customDimension.DimensionIndex));

            if (customMetrics?.Count > 0)
                foreach (var customMetric in customMetrics)
                    builder.Set(Convert.ToString(customMetric.MetricValue), Fields.CustomMetric((nuint)customMetric.MetricIndex));


            analyticsTracker.Send(builder.Build());
        }

        public void TrackException(
            string ExceptionMessage,
            bool isFatalException
        )
        {
            analyticsTracker.Send(DictionaryBuilder.CreateException(ExceptionMessage, isFatalException).Build());
        }

        public void TrackSocial(
            string socialNetworkName,
            string socialAction,
            string socialTarget
        )
        {
            analyticsTracker.Send(DictionaryBuilder.CreateSocial(socialNetworkName, socialAction, socialTarget).Build());
        }

        public void TrackTiming(
            string categroy,
            string label,
            long value,
            string variable
        )
        {
            analyticsTracker.Send(DictionaryBuilder.CreateTiming(categroy, new NSNumber(value), label, variable).Build());
        }

        public void SetCurrencyCode(string currencyCode)
        {
            if (!string.IsNullOrWhiteSpace(currencyCode))
                analyticsTracker.Set(GaiConstants.CurrencyCode, currencyCode);
        }

        public void TrackUserId(string userid)
        {
            if (!string.IsNullOrWhiteSpace(userid)) {
                analyticsTracker.Set(GaiConstants.UserId.ToString(), userid);
                var builder = DictionaryBuilder.CreateEvent("TrackUserId", "TrackUserId", "", new NSNumber(0));
                analyticsTracker.Send(builder.Build());
            }
        }

        EcommercePromotion generatePromotion(PromotionData prom)
        {
            var promotion = new EcommercePromotion();

            if (!string.IsNullOrEmpty(prom?.Id))
                promotion.SetId(prom.Id);

            if (!string.IsNullOrEmpty(prom?.Name))
                promotion.SetName(prom.Name);

            if (!string.IsNullOrEmpty(prom?.Creative))
                promotion.SetCreative(prom.Creative);

            if (!string.IsNullOrEmpty(prom?.Position))
                promotion.SetPosition(prom.Position);

            return promotion;
        }


        EcommerceProductAction generateCheckoutProductAction(ActionData actionData, ProductActions productAction)
        {
            var pAction = new EcommerceProductAction().SetAction(productAction.ToString());

            if (actionData != null) {
                if (string.IsNullOrEmpty(actionData?.Id)
                    && (productAction == ProductActions.purchase
                        || productAction == ProductActions.refund)) {
                    return pAction;
                } else if (!string.IsNullOrEmpty(actionData?.Id)) {
                    pAction.SetTransactionId(actionData.Id);
                }

                if (!string.IsNullOrEmpty(actionData?.Affiliation))
                    pAction.SetAffiliation(actionData.Affiliation);

                if (actionData?.Revenue != 0)
                    pAction.SetRevenue(new NSNumber(actionData.Revenue));

                if (actionData?.Tax != 0)
                    pAction.SetTax(new NSNumber(actionData.Tax));

                if (actionData?.Shipping != 0)
                    pAction.SetShipping(new NSNumber(actionData.Shipping));

                if (!string.IsNullOrEmpty(actionData?.Coupon))
                    pAction.SetCouponCode(actionData.Coupon);

                if (!string.IsNullOrEmpty(actionData?.List))
                    pAction.SetProductActionList(actionData.List);

                if (actionData?.Step != 0)
                    pAction.SetCheckoutStep(new NSNumber(actionData.Step));

                if (!string.IsNullOrEmpty(actionData?.Option))
                    pAction.SetCheckoutOption(actionData.Option);
            }

            return pAction;
        }
    }
}
