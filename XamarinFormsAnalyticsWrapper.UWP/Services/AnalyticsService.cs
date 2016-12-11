using GoogleAnalytics;
using GoogleAnalytics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Xamarin.Forms;
using XamarinFormsAnalyticsWrapper.Enums;
using XamarinFormsAnalyticsWrapper.Exceptions;
using XamarinFormsAnalyticsWrapper.Models;
using XamarinFormsAnalyticsWrapper.Services;
using XamarinFormsAnalyticsWrapper.UWP.Mapper;

[assembly: Dependency(typeof(XamarinFormsAnalyticsWrapper.UWP.Services.AnalyticsService))]
namespace XamarinFormsAnalyticsWrapper.UWP.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        const string allowTrackingKey = "AllowTracking";
        const long inMicrosMultiply = 1000000;

        static AnalyticsService thisRef;
        static Tracker analyticsTrackerInternal;
        static AnalyticsProductMapper mapper;
        static TrackerManager trackerManager;

        private string currencyCode = null;

        public bool OptOut
        {
            set
            {
                trackerManager.AppOptOut = value;
            }
            get
            {
                return trackerManager.AppOptOut;
            }
        }

        public double DispatchPeriod
        {
            set
            {
                GAServiceManager.Current.DispatchPeriod = TimeSpan.FromSeconds(value);
            }
            get
            {
                return GAServiceManager.Current.DispatchPeriod.TotalSeconds;
            }
        }

        public string ClientId
        {
            get
            {
                return trackerManager.DefaultTracker.UserId;
            }
        }

        public bool EnableAdvertisingFeatures
        {
            set
            {
            }
            get
            {
                return false;
            }
        }

        private Tracker analyticsTracker
        {
            set
            {
                analyticsTrackerInternal = value;
            }
            get
            {
                if (analyticsTrackerInternal == null)
                {
                    throw new ServiceNotInitializedException();
                }

                return analyticsTrackerInternal;
            }
        }

        public static AnalyticsService GetGASInstance()
        {
            if (thisRef == null)
            {
                thisRef = new AnalyticsService();
            }
            return thisRef;
        }

        public void Init(string gaTrackingId, int dispatchIntervalInSeconds = 5)
        {
            trackerManager = new TrackerManager(new GoogleAnalytics.Core.PlatformInfoProvider());
            GAServiceManager.Current.DispatchPeriod = TimeSpan.FromSeconds(dispatchIntervalInSeconds);
            analyticsTracker = trackerManager.GetTracker(gaTrackingId);
            
            mapper = new AnalyticsProductMapper();
        }

        public void ManualDispatch()
        {
            GAServiceManager.Current.Dispatch();
        }

        public void SetCurrencyCode(string currencyCode)
        {
            this.currencyCode = currencyCode;
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
            SetDimensionsAndMetrics(customDimensions, customMetrics);

            if (promotion != null)
                generatePromotion(promotion, campaignUrl);

            if (products == null) {

                analyticsTracker.SendView(screenName);
            } else {
                switch (productAction)
                {
                    case ProductActions.purchase:
                        var transaction = new Transaction {
                            TransactionId = actionData.Id,
                            Affiliation = actionData.Affiliation,
                            TotalTaxInMicros = Convert.ToInt64(actionData.Tax * inMicrosMultiply),
                            ShippingCostInMicros = Convert.ToInt64(actionData.Shipping * inMicrosMultiply),
                            TotalCostInMicros = Convert.ToInt64(actionData.Revenue * inMicrosMultiply),
                            CurrencyCode = currencyCode
                        };

                        foreach (var p in products)
                            transaction.Items.Add(mapper.mapProduct(p, actionData.Id, currencyCode));

                        analyticsTracker.SendTransaction(transaction);
                        break;
                    default:
                        analyticsTracker.SendView(screenName);
                        break;
                }
            }
        }

        private void generatePromotion(PromotionData prom, string campaignUrl)
        {
            if (!string.IsNullOrEmpty(prom?.Id))
                analyticsTracker.CampaignId = prom.Id;

            if (!string.IsNullOrEmpty(prom?.Name))
                analyticsTracker.CampaignName = prom.Name;

            if (!string.IsNullOrEmpty(prom?.Creative))
                analyticsTracker.CampaignKeyword = prom.Creative;

            if (!string.IsNullOrEmpty(prom?.Position))
                analyticsTracker.CampaignMedium = prom.Position;

            if (!string.IsNullOrEmpty(campaignUrl))
                analyticsTracker.CampaignSource = campaignUrl;
        }

        public void TrackEvent(
            EventData eventData,
            List<CustomDimension> customDimensions,
            List<CustomMetric> customMetrics
        )
        {
            SetDimensionsAndMetrics(customDimensions, customMetrics);

            analyticsTracker.SendEvent(
                eventData.EventCategory,
                eventData.EventAction,
                eventData.EventLabel,
                eventData.Id
                );
        }

        public void TrackException(
            string ExceptionMessage,
            bool isFatalException
        )
        {
            analyticsTracker.SendException(
                ExceptionMessage,
                isFatalException
                ); 
        }

        public void TrackSocial(
            string socialNetworkName,
            string socialAction,
            string socialTarget
        )
        {
            analyticsTracker.SendSocial(
                socialNetworkName,
                socialAction,
                socialTarget
                );
        }

        public void TrackTiming(
            string categroy,
            string label,
            long value,
            string variable
        )
        {
            analyticsTracker.SendTiming(
                TimeSpan.FromSeconds(value),
                categroy,
                variable,
                label
                );
        }

        public void TrackUserId(string userid)
        {
            if (!string.IsNullOrWhiteSpace(userid))
            {
                analyticsTracker.UserId = userid;

                analyticsTracker.SendEvent(
                    "TrackUserId",
                    "TrackUserId",
                    string.Empty,
                    0
                    );
            }
        }

        private void SetDimensionsAndMetrics(List<CustomDimension> customDimensions, List<CustomMetric> customMetrics)
        {
            if (customDimensions?.Count > 0)
                foreach (var customDimension in customDimensions)
                    analyticsTracker.SetCustomDimension(customDimension.DimensionIndex, customDimension.DimensionValue);

            if (customMetrics?.Count > 0)
                foreach (var customMetric in customMetrics)
                    analyticsTracker.SetCustomMetric(customMetric.MetricIndex, Convert.ToInt64(customMetric.MetricValue));
        }
    }
}
