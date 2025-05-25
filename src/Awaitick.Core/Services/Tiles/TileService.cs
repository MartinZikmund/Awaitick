//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Windows.ApplicationModel.Activation;
//using Windows.Data.Xml.Dom;
//using Windows.Globalization.DateTimeFormatting;
//using Windows.UI.Notifications;
//using Windows.UI.StartScreen;
//using Awaitick.Core.Models;
//using Awaitick.Core.Services;
//using Awaitick.Core.Services.Data;
//using Awaitick.Core.Services.Tile;
//using CommunityToolkit.Uwp.Notifications;

//namespace Awaitick.Core.Services
//{
//    public class TileService : ITileService
//    {
//        private readonly IStringLocalizer _localizationService;
//        private const string EventTileIdPrefix = "Tile_Countdown_";
//        private const string LaunchFormatString = "Countdown_{0}";
//        private const int ScheduleNotificationFutureSecondsLimit = 3;
//        private const int MaxScheduledNotificationIdLength = 15;
//        private const string EventTileIdFormatString = EventTileIdPrefix + "{0}";

//        private string GenerateEventTileId(string eventId)
//        {
//            return string.Format(EventTileIdFormatString, eventId);
//        }

//        public TileService(IStringLocalizer localizationService)
//        {
//            _localizationService = localizationService;
//        }

//        public async Task<bool> PinCountdownAsync(EventCountdown eventCountdown)
//        {
//            try
//            {
//                string displayName = eventCountdown.Name;
//                string tileId = GenerateEventTileId(eventCountdown.Id);
//                Uri square150X150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.png");
//                SecondaryTile secondaryTile = new SecondaryTile(tileId,
//                    displayName,
//                    string.Format(LaunchFormatString, eventCountdown.Id),
//                    square150X150Logo,
//                    TileSize.Square150x150);
//                secondaryTile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.png");
//                secondaryTile.VisualElements.Square310x310Logo = new Uri("ms-appx:///Assets/Square310Logo.png");
//                secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
//                secondaryTile.VisualElements.ShowNameOnSquare310x310Logo = true;
//                secondaryTile.VisualElements.ShowNameOnWide310x150Logo = true;
//                if (await secondaryTile.RequestCreateAsync())
//                {
//                    //TODO:Track and log

//                    return true;
//                }
//                else
//                {
//                    return false;
//                }
//            }
//            catch (Exception ex)
//            {
//                //TODO:Track and log

//                return false;
//            }
//        }

//        public async Task<bool> UnpinCountdownAsync(EventCountdown eventCountdown)
//        {
//            try
//            {
//                if (IsCountdownPinned(eventCountdown.Id))
//                {
//                    var tileId = GenerateEventTileId(eventCountdown.Id);
//                    SecondaryTile secondaryTile = new SecondaryTile(tileId);
//                    return await secondaryTile.RequestDeleteAsync();
//                }
//                return true;
//            }
//            catch (Exception ex)
//            {
//                //TODO:Track and log

//                return false;
//            }
//        }

//        public bool IsCountdownPinned(string id)
//        {
//            return SecondaryTile.Exists(GenerateEventTileId(id));
//        }

//        private XmlDocument GenerateTileXml(EventCountdown eventCountdown, bool forceFinish)
//        {
//            TileBindingContentAdaptive mediumBindingContent = new TileBindingContentAdaptive()
//            {
//                PeekImage = new TilePeekImage()
//                {
//                    Source = new TileImageSource(eventCountdown.BackgroundImagePath ?? "ms-appx:///Assets/Square150x150Logo.png"),
//                }
//            };

//            TileBindingContentAdaptive wideBindingContent = new TileBindingContentAdaptive()
//            {
//                PeekImage = new TilePeekImage()
//                {
//                    Source = new TileImageSource(eventCountdown.BackgroundImagePath ?? "ms-appx:///Assets/Wide310x150Logo.png"),
//                }
//            };

//            TileBindingContentAdaptive largeBindingContent = new TileBindingContentAdaptive()
//            {
//                PeekImage = new TilePeekImage()
//                {
//                    Source = new TileImageSource(eventCountdown.BackgroundImagePath ?? "ms-appx:///Assets/Square310Logo.png"),
//                }
//            };

//            var extendedCountdown = new CountdownViewModel(eventCountdown);
//            TileGroup group = null;
//            if (!extendedCountdown.Finished && !forceFinish)
//            {
//                group = CreateGroup(
//                    $"{extendedCountdown.DaysLeft} {_localizationService.Days}, {extendedCountdown.HoursLeft} {_localizationService.Hours}",
//                    extendedCountdown.TargetDateTime.ToString("f"));
//            }
//            else
//            {
//                group = CreateGroup(extendedCountdown.CelebrationMessage,
//                    extendedCountdown.TargetDateTime.ToString("f"));
//            }
//            mediumBindingContent.Children.Add(group);
//            largeBindingContent.Children.Add(group);
//            wideBindingContent.Children.Add(group);

//            TileBinding mediumBinding = new TileBinding()
//            {
//                Branding = TileBranding.NameAndLogo,

//                DisplayName = eventCountdown.Name,

//                Content = mediumBindingContent
//            };

//            TileBinding wideBinding = new TileBinding()
//            {
//                Branding = TileBranding.NameAndLogo,

//                DisplayName = eventCountdown.Name,

//                Content = wideBindingContent
//            };

//            TileBinding largeBinding = new TileBinding()
//            {
//                Branding = TileBranding.NameAndLogo,

//                DisplayName = eventCountdown.Name,

//                Content = largeBindingContent
//            };

//            TileContent content = new TileContent()
//            {
//                Visual = new TileVisual()
//                {
//                    TileMedium = mediumBinding,
//                    TileWide = wideBinding,
//                    TileLarge = largeBinding
//                }
//            };

//            return content.GetXml();
//        }

//        public void UpdateCountdownTile(EventCountdown eventCountdown)
//        {
//            try
//            {
//                if (IsCountdownPinned(eventCountdown.Id))
//                {
//                    // Generate WinRT notification
//                    var notification = new TileNotification(GenerateTileXml(eventCountdown, false));
//                    var updater =
//                        TileUpdateManager.CreateTileUpdaterForSecondaryTile(GenerateEventTileId(eventCountdown.Id));
//                    updater.EnableNotificationQueue(false);
//                    updater.Update(notification);
//                }
//            }
//            catch (Exception ex)
//            {
//                //TODO:Track and log

//            }
//        }

//        public void UpdateMainTile(params EventCountdown[] countdowns)
//        {
//            try
//            {
//                TileBindingContentAdaptive largeBindingContent = new TileBindingContentAdaptive()
//                {
//                    PeekImage = new TilePeekImage()
//                    {
//                        Source = new TileImageSource("ms-appx:///Assets/Square310Logo.png"),
//                    }
//                };

//                TileBindingContentAdaptive wideBindingContent = new TileBindingContentAdaptive()
//                {
//                    PeekImage = new TilePeekImage()
//                    {
//                        Source = new TileImageSource("ms-appx:///Assets/Wide310x150Logo.png"),
//                    }
//                };

//                TileBindingContentAdaptive mediumBindingContent = new TileBindingContentAdaptive()
//                {
//                    PeekImage = new TilePeekImage()
//                    {
//                        Source = new TileImageSource("ms-appx:///Assets/Square150x150Logo.png"),
//                    }
//                };

//                foreach (var countdown in countdowns.Take(3))
//                {
//                    var extendedCountdown = new CountdownViewModel(countdown);
//                    TileGroup group = null;
//                    TileGroup smallGroup = null;
//                    if (!extendedCountdown.Finished)
//                    {
//                        group = CreateGroup(extendedCountdown.Name,
//                            $"{extendedCountdown.DaysLeft} {_localizationService.Days}, {extendedCountdown.HoursLeft} {_localizationService.Hours}");
//                        smallGroup = CreateSmallGroup(extendedCountdown.Name,
//                            $"{extendedCountdown.DaysLeft} {_localizationService.Days}, {extendedCountdown.HoursLeft} {_localizationService.Hours}");
//                    }
//                    else
//                    {
//                        group = CreateGroup(extendedCountdown.Name, extendedCountdown.CelebrationMessage);
//                        smallGroup = CreateSmallGroup(extendedCountdown.Name, extendedCountdown.CelebrationMessage);
//                    }
//                    largeBindingContent.Children.Add(group);
//                    largeBindingContent.Children.Add(new TileText());
//                    mediumBindingContent.Children.Add(smallGroup);
//                    wideBindingContent.Children.Add(smallGroup);
//                }

//                TileBinding largeBinding = new TileBinding()
//                {
//                    Branding = TileBranding.Name,

//                    DisplayName = _localizationService.AppName,

//                    Content = largeBindingContent
//                };


//                TileBinding mediumBinding = new TileBinding()
//                {
//                    Branding = TileBranding.Name,

//                    DisplayName = _localizationService.ShortAppName,

//                    Content = mediumBindingContent
//                };

//                TileBinding wideBinding = new TileBinding()
//                {
//                    Branding = TileBranding.Name,

//                    DisplayName = _localizationService.AppName,

//                    Content = wideBindingContent
//                };

//                TileContent content = new TileContent()
//                {
//                    Visual = new TileVisual()
//                    {
//                        TileMedium = mediumBinding,
//                        TileWide = wideBinding,
//                        TileLarge = largeBinding
//                    }
//                };

//                XmlDocument doc = content.GetXml();

//                // Generate WinRT notification
//                var notification = new TileNotification(doc);
//                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
//                updater.EnableNotificationQueue(false);
//                updater.Clear();
//                updater.Update(notification);
//            }
//            catch (Exception ex)
//            {
//                //TODO:Track and log

//            }
//        }

//        public void ScheduleCountdownNotification(EventCountdown eventCountdown)
//        {
//            try
//            {
//                if (IsCountdownPinned(eventCountdown.Id))
//                {
//                    if ((eventCountdown.TargetDateTime - DateTimeOffset.Now).TotalSeconds >
//                        ScheduleNotificationFutureSecondsLimit)
//                    {
//                        // Generate WinRT notification                    
//                        ScheduledTileNotification scheduledTileNotification =
//                            new ScheduledTileNotification(GenerateTileXml(eventCountdown, true),
//                                eventCountdown.TargetDateTime)
//                            {
//                                Id = GenerateScheduleId(eventCountdown.Id)
//                            };
//                        TileUpdater manager =
//                            TileUpdateManager.CreateTileUpdaterForSecondaryTile(GenerateEventTileId(eventCountdown.Id));
//                        manager.AddToSchedule(scheduledTileNotification);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                //TODO:Track and log

//            }
//        }

//        private string GenerateScheduleId(string eventCountdownId)
//        {
//            return GuidToBase64(eventCountdownId);
//        }

//        private string GuidToBase64(string guid)
//        {
//            var converted = Convert.ToBase64String(new Guid(guid).ToByteArray()).Replace("/", "-").Replace("+", "_").Replace("=", "");
//            if (converted.Length > MaxScheduledNotificationIdLength)
//            {
//                return converted.Substring(0, MaxScheduledNotificationIdLength);
//            }
//            else
//            {
//                return converted;
//            }
//        }

//        public void UnscheduleCountdownNotification(EventCountdown eventCountdown)
//        {
//            try
//            {
//                if (IsCountdownPinned(eventCountdown.Id))
//                {
//                    TileUpdater manager =
//                        TileUpdateManager.CreateTileUpdaterForSecondaryTile(GenerateEventTileId(eventCountdown.Id));
//                    var scheduledNotifications = manager.GetScheduledTileNotifications();
//                    string removedId = GenerateScheduleId(eventCountdown.Id);
//                    var notification =
//                        (from n in scheduledNotifications where n.Id == removedId select n).FirstOrDefault();
//                    if (notification != null)
//                    {
//                        manager.RemoveFromSchedule(notification);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                //TODO:Track and log
//            }
//        }

//        private static TileGroup CreateGroup(string from, string subject)
//        {
//            return new TileGroup()
//            {
//                Children =
//        {
//            new TileSubgroup()
//            {
//                Children =
//                {
//                    new TileText()
//                    {
//                        Text = from,
//                        Style = TileTextStyle.Body,
//                        Wrap= true
//                    },

//                    new TileText()
//                    {
//                        Text = subject,
//                        Style = TileTextStyle.BodySubtle,
//                        Wrap= true
//                    },
//                }
//            }
//        }
//            };
//        }

//        private static TileGroup CreateSmallGroup(string from, string subject)
//        {
//            return new TileGroup()
//            {
//                Children =
//        {
//            new TileSubgroup()
//            {
//                Children =
//                {
//                    new TileText()
//                    {
//                        Text = from,
//                        Style = TileTextStyle.Caption,
//                        Wrap= true
//                    },

//                    new TileText()
//                    {
//                        Text = subject,
//                        Style = TileTextStyle.CaptionSubtle,
//                        Wrap= true
//                    },
//                }
//            }
//        }
//            };
//        }

//    }

//}
