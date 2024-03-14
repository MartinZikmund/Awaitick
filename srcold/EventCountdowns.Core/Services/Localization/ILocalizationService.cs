using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCountdowns.Core.Services
{
    public interface ILocalizationService
    {
        string this[string key] { get; }

        string GetString(string key);

        //specific keys

        string AppName { get; }

        string Add { get; }

        string Done { get; }

        string Manage { get; }

        string Save { get; }

        string Cancel { get; }

        string Delete { get; }


        string Edit { get; }

        string Share { get; }

        string Hours { get; }

        string Minutes { get; }

        string Seconds { get; }

        string Days { get; }

        string Yes { get; }

        string No { get; }

        string ConfirmDelete { get; }

        string AreYouSureDeleteTextFormat { get; }

        string Name { get; }

        string Date { get; }

        string Time { get; }

        string Background { get; }

        string PinToStart { get; }

        string UnpinFromStart { get; }

        string SetAsLockscreen { get; }

        string SharingFormatString { get; }

        string AppSocialHandle { get; }

        string AddEvent { get; }

        string EditEvent { get; }

        string AboutApp { get; }

        string MoreAppsBySphereline { get; }

        string RateThisApp { get; }

        string CopyrightNotice { get; }

        string Celebration { get; }

        string HappyEaster { get; }

        string MerryChristmas { get; }

        string ScaryHalloween { get; }

        string DefaultCelebration { get; }

        string SendFeedback { get; }

        string ChooseYourImage { get; }

        string DefaultBackgrounds { get; }

        string Selected { get; }

        string Christmas { get; }

        string Easter { get; }

        string Halloween { get; }

        string BuyMeCoffee { get; }

        string Preview { get; }

        string DoYouEnjoyAppFormatString { get; }

        string RatingDialogContentFormatString { get; }

        string RateNow { get; }

        string Later { get; }

        string DoNotRemindMe { get; }

        string MadeWith { get; }

        string InPrague { get; }

        string PricesInUsd { get; }

        string VentiCoffee { get; }

        string VentiCoffeePrice { get; }

        string GrandeCoffee { get; }

        string GrandeCoffeePrice { get; }

        string TallCoffee { get; }

        string TallCoffeePrice { get; }

        string TrentaCoffee { get; }

        string TrentaCoffeePrice { get; }

        string DonateDescription { get; }

        string ShortAppName { get; }

        string CoffeeThankYou { get; }

        string SharingFinishedEventFormatString { get; }
        string HappyNewYear { get; }
        string NewYear { get; }

        string PurchaseUnsuccessfulText { get; }
        string PurchaseUnsuccessful { get; }
    }
}
