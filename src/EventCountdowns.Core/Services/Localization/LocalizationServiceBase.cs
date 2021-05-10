using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCountdowns.Core.Services
{
    public abstract class LocalizationServiceBase : ILocalizationService
    {
        public abstract string this[string key] { get; }

        public string AppName => this[nameof(AppName)];

        public string Edit => this[nameof(Edit)];

        public string Add => this[nameof(Add)];

        public string Done => this[nameof(Done)];

        public string Manage => this[nameof(Manage)];

        public string Save => this[nameof(Save)];

        public string Cancel => this[nameof(Cancel)];

        public string Delete => this[nameof(Delete)];

        public string Share => this[nameof(Share)];

        public string Hours => this[nameof(Hours)];

        public string Days => this[nameof(Days)];

        public string Minutes => this[nameof(Minutes)];

        public string Seconds => this[nameof(Seconds)];

        public string Yes => this[nameof(Yes)];

        public string No => this[nameof(No)];

        public string ConfirmDelete => this[nameof(ConfirmDelete)];

        public string AreYouSureDeleteTextFormat => this[nameof(AreYouSureDeleteTextFormat)];

        public string Name => this[nameof(Name)];

        public string Date => this[nameof(Date)];

        public string Time => this[nameof(Time)];

        public string Background => this[nameof(Background)];

        public string PinToStart => this[nameof(PinToStart)];

        public string UnpinFromStart => this[nameof(UnpinFromStart)];

        public string SetAsLockscreen => this[nameof(SetAsLockscreen)];

        public string SharingFormatString => this[nameof(SharingFormatString)];

        public string AppSocialHandle => this[nameof(AppSocialHandle)];

        public string AddEvent => this[nameof(AddEvent)];

        public string EditEvent => this[nameof(EditEvent)];

        public string AboutApp => this[nameof(AboutApp)];

        public string MoreAppsBySphereline => this[nameof(MoreAppsBySphereline)];

        public string RateThisApp => this[nameof(RateThisApp)];

        public string CopyrightNotice => this[nameof(CopyrightNotice)];

        public string Celebration => this[nameof(Celebration)];

        public string HappyEaster => this[nameof(HappyEaster)];

        public string MerryChristmas => this[nameof(MerryChristmas)];

        public string ScaryHalloween => this[nameof(ScaryHalloween)];

        public string DefaultCelebration => this[nameof(DefaultCelebration)];

        public string SendFeedback => this[nameof(SendFeedback)];

        public string ChooseYourImage => this[nameof(ChooseYourImage)];

        public string DefaultBackgrounds => this[nameof(DefaultBackgrounds)];

        public string Selected => this[nameof(Selected)];

        public string Christmas => this[nameof(Christmas)];

        public string Easter => this[nameof(Easter)];

        public string Halloween => this[nameof(Halloween)];

        public string BuyMeCoffee => this[nameof(BuyMeCoffee)];

        public string Preview => this[nameof(Preview)];
        public string DoYouEnjoyAppFormatString => this[nameof(DoYouEnjoyAppFormatString)];
        public string RatingDialogContentFormatString => this[nameof(RatingDialogContentFormatString)];
        public string RateNow => this[nameof(RateNow)];
        public string Later => this[nameof(Later)];
        public string DoNotRemindMe => this[nameof(DoNotRemindMe)];

        public string MadeWith => this[nameof(MadeWith)];
        public string InPrague => this[nameof(InPrague)];

        public string PricesInUsd => this[nameof(PricesInUsd)];

        public string VentiCoffee => this[nameof(VentiCoffee)];

        public string VentiCoffeePrice => this[nameof(VentiCoffeePrice)];

        public string GrandeCoffee => this[nameof(GrandeCoffee)];

        public string GrandeCoffeePrice => this[nameof(GrandeCoffeePrice)];

        public string TallCoffee => this[nameof(TallCoffee)];

        public string TallCoffeePrice => this[nameof(TallCoffeePrice)];

        public string TrentaCoffee => this[nameof(TrentaCoffee)];

        public string TrentaCoffeePrice => this[nameof(TrentaCoffeePrice)];

        public string DonateDescription => this[nameof(DonateDescription)];

        public string ShortAppName => this[nameof(ShortAppName)];
        public string CoffeeThankYou => this[nameof(CoffeeThankYou)];

        public string SharingFinishedEventFormatString => this[nameof(SharingFinishedEventFormatString)];
        public string HappyNewYear => this[nameof(HappyNewYear)];
        public string NewYear => this[nameof(NewYear)];
        public string PurchaseUnsuccessfulText => this[nameof(PurchaseUnsuccessfulText)];
        public string PurchaseUnsuccessful => this[nameof(PurchaseUnsuccessful)];
    }
}