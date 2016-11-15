using System;
using System.Collections.Generic;
using XamarinFormsAnalyticsWrapper.Enums;
using XamarinFormsAnalyticsWrapper.Models;

namespace XamarinFormsAnalyticsWrapper.Services
{
    public interface IAnalyticsService
    {
        bool OptOut { set; get; }
        double DispatchPeriod { set; }
        string ClientId { get; }
        bool EnableAdvertisingFeatures { set; }
        void ManualDispatch ();
        void TrackScreen (string screenName, string campaignUrl, List<ProductData> products, ProductActions productAction, ActionData actionData, PromotionData promotion, List<CustomDimension> customDimensions, List<CustomMetric> customMetrics);
        void TrackEvent (EventData eventData, List<CustomDimension> customDimensions, List<CustomMetric> customMetrics);
        void TrackException (string ExceptionMessage, bool isFatalException);
        void TrackSocial (string socialNetworkName, string socialAction, string socialTarget);
        void TrackTiming (string categroy, string label, long value, string variable);
        void SetCurrencyCode (string currencyCode);
        void TrackUserId (string userid);
    }
}
