# Scheduled Notifications Service Implementation

## Overview

This document details the implementation of scheduled notifications for countdown completion across Windows, Android, and iOS platforms in the Awaitick app.

## Requirements

| Requirement | Details |
|-------------|---------|
| Trigger Time | Exactly at countdown's `TargetDateTime` |
| Background Operation | Works when app is closed |
| Notification Actions | Snooze (10 min) and Dismiss buttons |
| Deep Linking | Tap opens app and navigates to specific countdown |
| Platforms | Windows (WinUI), Android, iOS |
| Startup Sync | Re-schedule all countdowns on app launch |

---

## Architecture

### Service Layer

```
IScheduledNotificationService (Core - interface)
├── ScheduledNotificationService.Windows.cs  (Windows implementation)
├── ScheduledNotificationService.Android.cs  (Android implementation)
├── ScheduledNotificationService.iOS.cs      (iOS implementation)
└── ScheduledNotificationService.others.cs   (Fallback stub)

IDeepLinkService (Core - interface)
└── DeepLinkService.cs (Core - implementation)
```

### Interface Definition

**IScheduledNotificationService** (`src/Awaitick.Core/Services/ScheduledNotification/`)

```csharp
public interface IScheduledNotificationService
{
    // Existing methods
    void ScheduleCountdownNotification(EventCountdown eventCountdown);
    void UnscheduleCountdownNotification(EventCountdown eventCountdown);
    void SuppressCountdownNotification(EventCountdown eventCountdown);
    void UnSuppressAllCountdownNotifications();

    // New methods
    Task<bool> RequestPermissionAsync();
    bool HasPermission { get; }
    Task RescheduleAllNotificationsAsync(IEnumerable<EventCountdown> countdowns);
}
```

**IDeepLinkService** (`src/Awaitick.Core/Services/DeepLink/`)

```csharp
public interface IDeepLinkService
{
    void SetPendingNavigation(string countdownId);
    string? ConsumePendingNavigation();
    bool HasPendingNavigation { get; }
}
```

---

## Platform Implementations

### Windows (WinUI 3)

**Technology Stack:**
- `Microsoft.Toolkit.Uwp.Notifications` NuGet package (v7.1.3)
- `ScheduledToastNotification` from `Windows.UI.Notifications`
- `ToastNotificationManagerCompat` for activation handling

**File:** `src/Awaitick/Services/ScheduledNotification/ScheduledNotificationService.Windows.cs`

```csharp
#if WINDOWS
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;

public partial class ScheduledNotificationService : IScheduledNotificationService
{
    public void ScheduleCountdownNotification(EventCountdown eventCountdown)
    {
        if ((eventCountdown.TargetDateTime - DateTimeOffset.Now).TotalSeconds <= 3)
            return;

        UnscheduleCountdownNotification(eventCountdown);

        var content = new ToastContentBuilder()
            .AddArgument("action", "viewCountdown")
            .AddArgument("countdownId", eventCountdown.Id)
            .AddText(eventCountdown.Name)
            .AddText(eventCountdown.CelebrationMessage)
            .AddAudio(new Uri("ms-winsoundevent:Notification.Reminder"))
            .SetToastScenario(ToastScenario.Reminder)
            .AddButton(new ToastButtonSnooze())
            .AddButton(new ToastButtonDismiss())
            .GetToastContent();

        var notification = new ScheduledToastNotification(
            content.GetXml(),
            eventCountdown.TargetDateTime)
        {
            Id = GenerateNotificationId(eventCountdown.Id),
            Tag = "Countdown",
            Group = eventCountdown.Id
        };

        ToastNotificationManagerCompat.CreateToastNotifier()
            .AddToSchedule(notification);
    }

    private static string GenerateNotificationId(string countdownId)
    {
        // GUID to Base64, max 15 chars for Windows
        var bytes = new Guid(countdownId).ToByteArray();
        return Convert.ToBase64String(bytes)
            .Replace("/", "-")
            .Replace("+", "_")
            .Replace("=", "")[..15];
    }
}
#endif
```

**Toast Activation Handler** (in `App.xaml.cs`):

```csharp
#if WINDOWS
public CountdownsApp()
{
    this.InitializeComponent();
    ToastNotificationManagerCompat.OnActivated += OnToastActivated;
}

private void OnToastActivated(ToastNotificationActivatedEventArgsCompat e)
{
    var args = ToastArguments.Parse(e.Argument);
    if (args.TryGetValue("countdownId", out var countdownId))
    {
        var deepLinkService = Host?.Services.GetRequiredService<IDeepLinkService>();
        deepLinkService?.SetPendingNavigation(countdownId);

        MainWindow?.DispatcherQueue.TryEnqueue(() =>
        {
            // Navigation will be handled by MainViewModel
        });
    }
}
#endif
```

**NuGet Package Reference** (in `Awaitick.csproj`):

```xml
<ItemGroup Condition="$(TargetFramework.Contains('windows'))">
    <PackageReference Include="Microsoft.Toolkit.Uwp.Notifications" />
</ItemGroup>
```

---

### Android

**Technology Stack:**
- `AlarmManager` with `setExactAndAllowWhileIdle()` for scheduling
- `BroadcastReceiver` for alarm handling
- `NotificationManager` with notification channels
- `PendingIntent` for notification actions

**Required Permissions** (`AndroidManifest.xml`):

```xml
<uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
<uses-permission android:name="android.permission.SCHEDULE_EXACT_ALARM" />
<uses-permission android:name="android.permission.USE_EXACT_ALARM" />
<uses-permission android:name="android.permission.RECEIVE_BOOT_COMPLETED" />
<uses-permission android:name="android.permission.VIBRATE" />
```

**Receiver Registration** (`AndroidManifest.xml`):

```xml
<application ...>
    <receiver android:name="Awaitick.Droid.NotificationAlarmReceiver"
              android:exported="false" />
    <receiver android:name="Awaitick.Droid.BootReceiver"
              android:exported="false">
        <intent-filter>
            <action android:name="android.intent.action.BOOT_COMPLETED" />
        </intent-filter>
    </receiver>
</application>
```

**File:** `src/Awaitick/Platforms/Android/NotificationAlarmReceiver.cs`

```csharp
#if __ANDROID__
[BroadcastReceiver(Enabled = true, Exported = false)]
public class NotificationAlarmReceiver : BroadcastReceiver
{
    public const string ExtraCountdownId = "countdown_id";
    public const string ExtraCountdownName = "countdown_name";
    public const string ExtraCountdownMessage = "countdown_message";
    public const string ActionSnooze = "dev.mzikmund.awaitick.SNOOZE";
    public const string ActionDismiss = "dev.mzikmund.awaitick.DISMISS";

    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context == null || intent == null) return;

        var action = intent.Action;
        var countdownId = intent.GetStringExtra(ExtraCountdownId);

        if (action == ActionSnooze)
        {
            HandleSnooze(context, countdownId);
            return;
        }

        if (action == ActionDismiss)
        {
            DismissNotification(context, countdownId);
            return;
        }

        ShowNotification(context, intent);
    }

    private void ShowNotification(Context context, Intent intent)
    {
        var countdownId = intent.GetStringExtra(ExtraCountdownId) ?? "";
        var name = intent.GetStringExtra(ExtraCountdownName) ?? "Countdown";
        var message = intent.GetStringExtra(ExtraCountdownMessage) ?? "";
        var notificationId = countdownId.GetHashCode();

        // Create tap intent
        var tapIntent = new Intent(context, typeof(MainActivity));
        tapIntent.SetAction(Intent.ActionView);
        tapIntent.PutExtra(ExtraCountdownId, countdownId);
        tapIntent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);

        var tapPendingIntent = PendingIntent.GetActivity(
            context, notificationId, tapIntent,
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

        // Create snooze/dismiss intents...

        var builder = new NotificationCompat.Builder(context, "countdown_notifications")
            .SetSmallIcon(Resource.Mipmap.icon)
            .SetContentTitle(name)
            .SetContentText(message)
            .SetPriority(NotificationCompat.PriorityHigh)
            .SetCategory(NotificationCompat.CategoryReminder)
            .SetAutoCancel(true)
            .SetContentIntent(tapPendingIntent)
            .AddAction(0, "Snooze", snoozePendingIntent)
            .AddAction(0, "Dismiss", dismissPendingIntent);

        NotificationManagerCompat.From(context).Notify(notificationId, builder.Build());
    }
}
#endif
```

**File:** `src/Awaitick/Services/ScheduledNotification/ScheduledNotificationService.Android.cs`

```csharp
#if __ANDROID__
public partial class ScheduledNotificationService : IScheduledNotificationService
{
    private readonly Context _context;
    private readonly AlarmManager _alarmManager;

    public ScheduledNotificationService()
    {
        _context = Android.App.Application.Context;
        _alarmManager = (AlarmManager)_context.GetSystemService(Context.AlarmService)!;
        CreateNotificationChannel();
    }

    public void ScheduleCountdownNotification(EventCountdown eventCountdown)
    {
        var triggerTime = eventCountdown.TargetDateTime.ToUnixTimeMilliseconds();
        if (triggerTime <= DateTimeOffset.Now.ToUnixTimeMilliseconds()) return;

        UnscheduleCountdownNotification(eventCountdown);

        var intent = new Intent(_context, typeof(NotificationAlarmReceiver));
        intent.PutExtra(NotificationAlarmReceiver.ExtraCountdownId, eventCountdown.Id);
        intent.PutExtra(NotificationAlarmReceiver.ExtraCountdownName, eventCountdown.Name);
        intent.PutExtra(NotificationAlarmReceiver.ExtraCountdownMessage,
            eventCountdown.CelebrationMessage);

        var pendingIntent = PendingIntent.GetBroadcast(
            _context,
            eventCountdown.Id.GetHashCode(),
            intent,
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

        _alarmManager.SetExactAndAllowWhileIdle(
            AlarmType.RtcWakeup,
            triggerTime,
            pendingIntent);
    }
}
#endif
```

**MainActivity Deep Link Handling:**

```csharp
[Activity(
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,  // Important!
    ConfigurationChanges = ...)]
public class MainActivity : Microsoft.UI.Xaml.ApplicationActivity
{
    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);
        HandleIntent(intent);
    }

    private void HandleIntent(Intent? intent)
    {
        var countdownId = intent?.GetStringExtra(
            NotificationAlarmReceiver.ExtraCountdownId);
        if (!string.IsNullOrEmpty(countdownId))
        {
            IoC.GetService<IDeepLinkService>()?.SetPendingNavigation(countdownId);
        }
    }
}
```

---

### iOS

**Technology Stack:**
- `UNUserNotificationCenter` for scheduling
- `UNCalendarNotificationTrigger` with date components
- `UNNotificationAction` and `UNNotificationCategory` for actions
- `UNUserNotificationCenterDelegate` for handling responses

**File:** `src/Awaitick/Platforms/iOS/NotificationDelegate.cs`

```csharp
#if __IOS__
using UserNotifications;

public class NotificationDelegate : UNUserNotificationCenterDelegate
{
    public override void DidReceiveNotificationResponse(
        UNUserNotificationCenter center,
        UNNotificationResponse response,
        Action completionHandler)
    {
        var userInfo = response.Notification.Request.Content.UserInfo;
        var countdownId = userInfo["countdownId"]?.ToString();

        if (response.ActionIdentifier == "SNOOZE_ACTION")
        {
            HandleSnooze(countdownId);
        }
        else if (!string.IsNullOrEmpty(countdownId))
        {
            IoC.GetService<IDeepLinkService>()?.SetPendingNavigation(countdownId);
        }

        completionHandler();
    }

    public override void WillPresentNotification(
        UNUserNotificationCenter center,
        UNNotification notification,
        Action<UNNotificationPresentationOptions> completionHandler)
    {
        // Show notification even when app is in foreground
        completionHandler(UNNotificationPresentationOptions.Banner |
                         UNNotificationPresentationOptions.Sound);
    }
}
#endif
```

**File:** `src/Awaitick/Services/ScheduledNotification/ScheduledNotificationService.iOS.cs`

```csharp
#if __IOS__
using UserNotifications;

public partial class ScheduledNotificationService : IScheduledNotificationService
{
    public ScheduledNotificationService()
    {
        RegisterNotificationCategories();
    }

    private void RegisterNotificationCategories()
    {
        var snoozeAction = UNNotificationAction.FromIdentifier(
            "SNOOZE_ACTION", "Snooze", UNNotificationActionOptions.None);

        var dismissAction = UNNotificationAction.FromIdentifier(
            "DISMISS_ACTION", "Dismiss", UNNotificationActionOptions.Destructive);

        var category = UNNotificationCategory.FromIdentifier(
            "COUNTDOWN_CATEGORY",
            new[] { snoozeAction, dismissAction },
            Array.Empty<string>(),
            UNNotificationCategoryOptions.CustomDismissAction);

        UNUserNotificationCenter.Current.SetNotificationCategories(
            new NSSet<UNNotificationCategory>(category));
    }

    public void ScheduleCountdownNotification(EventCountdown eventCountdown)
    {
        if (eventCountdown.TargetDateTime <= DateTimeOffset.Now) return;

        UnscheduleCountdownNotification(eventCountdown);

        var content = new UNMutableNotificationContent
        {
            Title = eventCountdown.Name,
            Body = eventCountdown.CelebrationMessage ?? "",
            Sound = UNNotificationSound.Default,
            CategoryIdentifier = "COUNTDOWN_CATEGORY",
            UserInfo = new NSDictionary(
                new NSString("countdownId"),
                new NSString(eventCountdown.Id))
        };

        var target = eventCountdown.TargetDateTime;
        var dateComponents = new NSDateComponents
        {
            Year = target.Year,
            Month = target.Month,
            Day = target.Day,
            Hour = target.Hour,
            Minute = target.Minute,
            Second = target.Second
        };

        var trigger = UNCalendarNotificationTrigger.CreateTrigger(dateComponents, false);
        var request = UNNotificationRequest.FromIdentifier(
            eventCountdown.Id, content, trigger);

        UNUserNotificationCenter.Current.AddNotificationRequest(request, null);
    }
}
#endif
```

**Register Delegate** (in `Main.iOS.cs`):

```csharp
UNUserNotificationCenter.Current.Delegate = new NotificationDelegate();

var host = UnoPlatformHostBuilder.Create()
    .App(() => new CountdownsApp())
    .UseAppleUIKit()
    .Build();

host.Run();
```

---

## Deep Link Navigation Flow

```
┌─────────────────┐     ┌──────────────────┐     ┌─────────────────┐
│  Notification   │────▶│  Platform Layer  │────▶│ IDeepLinkService│
│  Tap/Action     │     │  (Activation)    │     │ SetPending()    │
└─────────────────┘     └──────────────────┘     └────────┬────────┘
                                                          │
                        ┌──────────────────┐              │
                        │  MainViewModel   │◀─────────────┘
                        │  ViewNavigatedTo │
                        └────────┬─────────┘
                                 │
                        ┌────────▼─────────┐
                        │  Navigate to     │
                        │  CountdownDetail │
                        └──────────────────┘
```

**MainViewModel Integration:**

```csharp
public override async void ViewNavigatedTo(object? parameter)
{
    // ... existing code ...

    // Handle pending deep link
    var pendingId = _deepLinkService.ConsumePendingNavigation();
    if (!string.IsNullOrEmpty(pendingId))
    {
        _navigationService.Navigate<CountdownDetailViewModel>(
            new CountdownDetailViewModel.NavigationModel(pendingId));
    }
}
```

---

## Startup Synchronization

**In `App.xaml.cs` OnLaunched:**

```csharp
protected override async void OnLaunched(LaunchActivatedEventArgs args)
{
    // ... existing initialization ...

    await Host.Services.GetRequiredService<IDataService>().InitializeAsync();

    // Reschedule all future notifications
    var dataService = Host.Services.GetRequiredService<IDataService>();
    var notificationService = Host.Services.GetRequiredService<IScheduledNotificationService>();
    var countdowns = await dataService.GetCountdownsAsync();
    await notificationService.RescheduleAllNotificationsAsync(
        countdowns.Where(c => c.TargetDateTime > DateTimeOffset.Now));

    // ... rest of initialization ...
}
```

---

## Files Summary

### New Files to Create

| File | Purpose |
|------|---------|
| `src/Awaitick.Core/Services/DeepLink/IDeepLinkService.cs` | Deep link interface |
| `src/Awaitick.Core/Services/DeepLink/DeepLinkService.cs` | Deep link implementation |
| `src/Awaitick/Services/ScheduledNotification/NotificationConstants.cs` | Shared constants |
| `src/Awaitick/Services/ScheduledNotification/ScheduledNotificationService.Windows.cs` | Windows impl |
| `src/Awaitick/Services/ScheduledNotification/ScheduledNotificationService.Android.cs` | Android impl |
| `src/Awaitick/Services/ScheduledNotification/ScheduledNotificationService.iOS.cs` | iOS impl |
| `src/Awaitick/Platforms/Android/NotificationAlarmReceiver.cs` | Android alarm handler |
| `src/Awaitick/Platforms/Android/BootReceiver.cs` | Android boot handler |
| `src/Awaitick/Platforms/iOS/NotificationDelegate.cs` | iOS notification delegate |

### Files to Modify

| File | Changes |
|------|---------|
| `src/Awaitick.Core/Services/ScheduledNotification/IScheduledNotificationService.cs` | Add new methods |
| `src/Awaitick.Core/Services/ScheduledNotification/ScheduledNotificationService.others.cs` | Update stub |
| `src/Awaitick/App.xaml.cs` | Toast handler, DI registration, startup sync |
| `src/Awaitick/Platforms/Android/AndroidManifest.xml` | Add permissions and receivers |
| `src/Awaitick/Platforms/Android/MainActivity.Android.cs` | Intent handling |
| `src/Awaitick/Platforms/iOS/Main.iOS.cs` | Register notification delegate |
| `src/Awaitick.Core/ViewModels/MainViewModel.cs` | Handle pending deep link |
| `src/Directory.Packages.props` | Add Windows toolkit package |
| `src/Awaitick/Awaitick.csproj` | Add conditional package reference |

---

## Edge Cases

| Scenario | Handling |
|----------|----------|
| Past date | Skip scheduling silently |
| Permission denied | Return false from `RequestPermissionAsync()` |
| Device reboot | BootReceiver (Android) + reschedule on startup (all) |
| App already running | `OnNewIntent` (Android), immediate navigation |
| Time zone change | `DateTimeOffset` handles conversion, reschedule on startup |
| Multiple notifications | Each gets unique ID based on countdown GUID |
| Countdown deleted | `UnscheduleCountdownNotification()` cancels pending notification |
| Countdown updated | Unschedule old, schedule new |

---

## Testing Checklist

- [ ] Schedule notification for future countdown
- [ ] Verify notification appears at exact time
- [ ] Tap notification (app closed) - opens to countdown detail
- [ ] Tap notification (app open) - navigates to countdown detail
- [ ] Snooze action - re-notifies after 10 minutes
- [ ] Dismiss action - clears notification
- [ ] Delete countdown - no notification fires
- [ ] Update countdown date - new notification at new time
- [ ] Device reboot (Android) - notifications still fire
- [ ] Multiple concurrent countdowns
- [ ] Past countdown - no notification scheduled
