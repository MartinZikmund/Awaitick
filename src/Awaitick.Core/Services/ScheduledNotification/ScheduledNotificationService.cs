#if WINDOWS_UWP
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Windows.UI.Notifications;
//using Awaitick.Core.Models;
//using Awaitick.Core.Services.ScheduledNotification;
//using CommunityToolkit.Uwp.Notifications;

//namespace Awaitick.Core.Services
//{
//    public class ScheduledNotificationService : IScheduledNotificationService
//    {
//        private const int ScheduleNotificationFutureSecondsLimit = 3;
//        private const int MaxScheduledNotificationIdLength = 15;
//        private const string LaunchFormatString = "Countdown_{0}";

//        private string GenerateToastId(string eventCountdownId)
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

//        public void ScheduleCountdownNotification(EventCountdown eventCountdown)
//        {
//            try
//            {
//                if ((eventCountdown.TargetDateTime - DateTimeOffset.Now).TotalSeconds >
//                    ScheduleNotificationFutureSecondsLimit)
//                {
//                    ToastContent content = new ToastContent()
//                    {
//                        Actions = new ToastActionsSnoozeAndDismiss(),
//                        ActivationType = ToastActivationType.Foreground,
//                        Audio = new ToastAudio()
//                        {
//                            Src = new Uri("ms-winsoundevent:Notification.Reminder")
//                        },
//                        Duration = ToastDuration.Long,
//                        Scenario = ToastScenario.Reminder,
//                        Launch = string.Format(LaunchFormatString, eventCountdown.Id),
//                        Visual = new ToastVisual()
//                        {

//                            TitleText = new ToastText()
//                            {
//                                Text = eventCountdown.Name
//                            },
//                            BodyTextLine1 = new ToastText()
//                            {
//                                Text = eventCountdown.CelebrationMessage
//                            }
//                        }
//                    };
//                    if (eventCountdown.BackgroundImagePath != null)
//                    {
//                        content.Visual.InlineImages.Add(new ToastImage()
//                        {
//                            Source = new ToastImageSource(eventCountdown.BackgroundImagePath)
//                        });
//                    }

//                    ScheduledToastNotification notification = new ScheduledToastNotification(content.GetXml(),
//                        eventCountdown.TargetDateTime);
//                    var id = GenerateToastId(eventCountdown.Id);
//                    notification.Id = id;
//                    var toastNotificationManager = ToastNotificationManager.CreateToastNotifier();
//                    toastNotificationManager.AddToSchedule(notification);
//                }
//            }
//            catch (Exception ex)
//            {
//                //TODO:Track
//            }
//        }

//        public void UnscheduleCountdownNotification(EventCountdown eventCountdown)
//        {
//            try
//            {
//                var toastNotificationManager = ToastNotificationManager.CreateToastNotifier();
//                var scheduledNotifications = toastNotificationManager.GetScheduledToastNotifications();
//                string removedId = GenerateToastId(eventCountdown.Id);
//                var notification = (from n in scheduledNotifications where n.Id == removedId select n).FirstOrDefault();
//                if (notification != null)
//                {
//                    toastNotificationManager.RemoveFromSchedule(notification);
//                }
//            }
//            catch (Exception ex)
//            {
//                //TODO:Track
//            }
//        }

//        public void SuppressCountdownNotification(EventCountdown eventCountdown)
//        {
//            try
//            {
//                var toastNotificationManager = ToastNotificationManager.CreateToastNotifier();
//                var scheduledNotifications = toastNotificationManager.GetScheduledToastNotifications();
//                string removedId = GenerateToastId(eventCountdown.Id);
//                var notification = (from n in scheduledNotifications where n.Id == removedId select n).FirstOrDefault();
//                if (notification != null)
//                {
//                    toastNotificationManager.RemoveFromSchedule(notification);
//                    notification.SuppressPopup = true;
//                    toastNotificationManager.AddToSchedule(notification);
//                }
//            }
//            catch (Exception ex)
//            {
//                //TODO:Track
//            }
//        }

//        public void UnSuppressAllCountdownNotifications()
//        {
//            try
//            {
//                var toastNotificationManager = ToastNotificationManager.CreateToastNotifier();
//                var scheduledNotifications = toastNotificationManager.GetScheduledToastNotifications();
//                foreach (var notification in scheduledNotifications)
//                {
//                    toastNotificationManager.RemoveFromSchedule(notification);
//                    notification.SuppressPopup = false;
//                    toastNotificationManager.AddToSchedule(notification);
//                }
//            }
//            catch (Exception ex)
//            {
//                //TODO:Track
//            }
//        }
//    }
//}
#endif
