using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;
using System.Threading.Tasks;
using XamarinFormsAnalyticsWrapper.Services;
using XamarinFormsAnalyticsWrapper.Models;
using XamarinFormsAnalyticsWrapper.Enums;

namespace AnalyticsSample {
    public class App : Application {
        Label trackAppCampaign;
        Label trackAppEvent;
        Label trackAppPage;
        Label trackAppPageWithProduct;
        Label trackAppPageWithProductAndCheckoutOption;
        Label trackAppPageWithProductAndCheckoutStep;
        Label trackAppException;
        Label trackAppSocial;
        Label trackAppPurchaseTransaction;
        Label trackAppTiming;

        public App() {
            var randomTrackingString = RandomString(4);
            var trackButton = new Button {
                Text = "Start tracking"
            };
            trackAppCampaign = new Label {
                Text = "Track campaign"
            };
            trackAppEvent = new Label {
                Text = "Track event"
            };
            trackAppPage = new Label {
                Text = "Track page"
            };
            trackAppPageWithProduct = new Label {
                Text = "Track page with products"
            };
            trackAppPageWithProductAndCheckoutOption = new Label {
                Text = "Track page with products and checkoutoption = creditcard"
            };
            trackAppPageWithProductAndCheckoutStep = new Label {
                Text = "Track page with products and checkoutstep = 1"
            };
            trackAppException = new Label {
                Text = "Track exception"
            };
            trackAppSocial = new Label {
                Text = "Track social"
            };
            trackAppPurchaseTransaction = new Label {
                Text = "Track purchase transaction"
            };
            trackAppTiming = new Label {
                Text = "Track timing"
            };

            trackButton.Clicked += async (sender, e) => {
                trackButton.IsEnabled = false;
                trackButton.Text = "Please wait..";
                await sampletracking(randomTrackingString);
                trackButton.Text = "Done with: " + randomTrackingString;
            };

            MainPage = new ContentPage {
                Content = new StackLayout {
                    Padding = 10,
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                        new Label {
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Xamarin Forms Analytics Wrapper Sample!"
                        },
                        new Label {
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Track with: " + randomTrackingString
                        },
                        trackButton,
                        trackAppCampaign,
                        trackAppEvent,
                        trackAppPage,
                        trackAppPageWithProduct,
                        trackAppPageWithProductAndCheckoutOption,
                        trackAppPageWithProductAndCheckoutStep,
                        trackAppException,
                        trackAppSocial,
                        trackAppPurchaseTransaction,
                        trackAppTiming
                    }
                }
            };
        }

        protected override void OnStart() {
            // Handle when your app starts
        }

        protected override void OnSleep() {
            // Handle when your app sleeps
        }

        protected override void OnResume() {
            // Handle when your app resumes
        }

        private async Task sampletracking(string randomTrackingString) {
            //get tracker
            var gaService = DependencyService.Get<IAnalyticsService>();

            gaService.TrackUserId("UserID_" + randomTrackingString);

            await Task.Delay(2000);

            //example GAProduct list
            var gaProductList = new List<ProductData>();
            gaProductList.Add(
                new ProductData {
                    Id = "ID_" + randomTrackingString,
                    Name = "Product " + randomTrackingString,
                    Brand = "rebuy",
                    Category = "Category 2",
                    Coupon = "123456",
                    Position = 1,
                    Price = 1.23,
                    Quantity = 1,
                    Variant = "Blue"
                }
            );

            //example Custom Dimension
            var customDimensions = new List<CustomDimension> {
                new CustomDimension {
                     DimensionIndex = 1,
                     DimensionValue = "Dimension 1 Value"
                },
                new CustomDimension {
                     DimensionIndex = 2,
                     DimensionValue = "Dimension 2 Value"
                },
            };

            //example Custom Metric
            var customMetrics = new List<CustomMetric> {
                new CustomMetric {
                     MetricIndex = 1,
                     MetricValue = 1.3F
                }
            };

            //track campaign
            gaService.TrackScreen("CampainPage",
                                  "http://examplepetstore.com/index.html?" +
                                  "utm_source=direct&utm_medium=" +
                                  "marketing_" + Device.OnPlatform("ios", "android", "winphone") +
                                  "&utm_campaign=capaingn_" + randomTrackingString +
                                  "&utm_content=email_variation_1",
                                  null,
                                  ProductActions.none,
                                  null,
                                  null,
                                  null,
                                  null
                                 );

            trackAppCampaign.TextColor = Color.Green;

            await Task.Delay(5000); //otherwise it will be to fast!

            //track event
            gaService.TrackEvent(
                new EventData {
                    EventAction = "Action_" + randomTrackingString,
                    EventCategory = "Event Category" + randomTrackingString,
                    EventLabel = "Event Label" + randomTrackingString,
                    Id = 1
                },
                null,
                null
            );
            trackAppEvent.TextColor = Color.Green;

            await Task.Delay(5000);

            //track page with custom Dimension/Metric and without product 
            gaService.TrackScreen(
                "Pagename no product",
                null,
                null,
                ProductActions.none,
                null,
                null,
                customDimensions,
                customMetrics
            );
            trackAppPage.TextColor = Color.Green;

            await Task.Delay(5000);

            //track page with product and productaction
            gaService.TrackScreen(
                "Pagename w/ product",
                null,
                gaProductList,
                ProductActions.detail,
                null,
                null,
                null,
                null
            );
            trackAppPageWithProduct.TextColor = Color.Green;

            await Task.Delay(5000);

            //track page with product and productaction and checkoutoption
            gaService.TrackScreen(
                "Pagename w/ product",
                null,
                gaProductList,
                ProductActions.checkout_option,
                new ActionData {
                    Step = 2,
                    Option = "Visa"
                },
                null,
                null,
                null
            );
            trackAppPageWithProductAndCheckoutOption.TextColor = Color.Green;

            await Task.Delay(5000);

            //track event with productaction, promotion and checkoutstep
            gaService.TrackScreen(
                "Checkout",
                null, gaProductList,
                ProductActions.detail,
                new ActionData {
                    Coupon = "Coupon123",
                    Step = 3,
                    Option = "MasterCard"
                },
                new PromotionData {
                    Id = "PROMO123",
                    Creative = "Banner 23",
                    Name = "Winter sale",
                    Position = "Slot 1"
                },
                null,
                null
            );
            trackAppPageWithProductAndCheckoutStep.TextColor = Color.Green;

            await Task.Delay(5000);

            //track nonfatal execption
            gaService.TrackException("Exception_" + randomTrackingString, false);
            trackAppException.TextColor = Color.Green;

            await Task.Delay(5000);

            //track social event
            gaService.TrackSocial("MySocialNetwork", "Action_" + randomTrackingString, "Target");
            trackAppSocial.TextColor = Color.Green;

            await Task.Delay(5000);

            //track purchase transaction 
            gaService.TrackScreen(
                "Transaction Page",
                null,
                gaProductList,
                ProductActions.purchase,
                new ActionData {
                    Id = "1337",
                    Step = 5,
                    Revenue = 12.33,
                    Shipping = 3.99,
                    Tax = 1.32
                },
                null,
                null,
                null
            );

            trackAppPurchaseTransaction.TextColor = Color.Green;

            await Task.Delay(5000);

            //track timing
            gaService.TrackTiming("timingCategory", "timingLabel_" + randomTrackingString, 1, "timingVariable");
            trackAppTiming.TextColor = Color.Green;
        }

        private static string RandomString(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
