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

NotificationConstants.cs (Core - shared constants)

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
- `ScheduledToastNotification` from `Windows.UI.Notifications`
- Manual XML toast construction for WinUI 3 compatibility
- `Windows.Data.Xml.Dom.XmlDocument` for toast XML

**File:** `src/Awaitick/Services/ScheduledNotification/ScheduledNotificationService.Windows.cs`

```csharp
#if WINDOWS
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

public class ScheduledNotificationService : IScheduledNotificationService
{
    public void ScheduleCountdownNotification(EventCountdown eventCountdown)
    {
        if ((eventCountdown.TargetDateTime - DateTimeOffset.Now).TotalSeconds <= 3)
            return;

        UnscheduleCountdownNotification(eventCountdown);

        var toastXml = BuildToastXml(eventCountdown);

        var notification = new ScheduledToastNotification(toastXml, eventCountdown.TargetDateTime)
        {
            Id = GenerateNotificationId(eventCountdown.Id),
            Tag = "Countdown",
            Group = eventCountdown.Id,
            SuppressPopup = _suppressedNotifications.Contains(eventCountdown.Id)
        };

        ToastNotificationManager.CreateToastNotifier()
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
private void HandleToastActivation(string launchArgs)
{
    // Parse launch arguments (format: "action=viewCountdown&countdownId=guid")
    var queryParams = launchArgs.Split('&')
        .Select(p => p.Split('='))
        .Where(p => p.Length == 2)
        .ToDictionary(p => p[0], p => p[1]);

    if (queryParams.TryGetValue(NotificationConstants.CountdownIdKey, out var countdownId))
    {
        var deepLinkService = Host?.Services.GetService<IDeepLinkService>();
        deepLinkService?.SetPendingNavigation(countdownId);

        MainWindow?.DispatcherQueue.TryEnqueue(() =>
        {
            var messenger = Host?.Services.GetService<IMessenger>();
            messenger?.Send(new DeepLinkReceivedMessage());
        });
    }
}
#endif
```

---

### Android

**Technology Stack:**
- `AlarmManager` with `setExactAndAllowWhileIdle()` for scheduling
- `BroadcastReceiver` for alarm handling
- `NotificationManager` with notification channels
- `PendingIntent` for notification actions
- Stable hash IDs via `NotificationConstants.GetStableId()` for consistent PendingIntent request codes

**Required Permissions** (`AndroidManifest.xml`):

```xml
<uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
<uses-permission android:name="android.permission.SCHEDULE_EXACT_ALARM" />
<uses-permission android:name="android.permission.USE_EXACT_ALARM" />
<uses-permission android:name="android.permission.VIBRATE" />
<uses-permission android:name="android.permission.WAKE_LOCK" />
```

**Receiver Registration:**

`NotificationAlarmReceiver` is registered via `[BroadcastReceiver]` attribute in code (not in AndroidManifest.xml).

**File:** `src/Awaitick/Platforms/Android/NotificationAlarmReceiver.cs`

```csharp
#if __ANDROID__
[BroadcastReceiver(Enabled = true, Exported = false)]
public class NotificationAlarmReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        // Handles alarm broadcasts, snooze, and dismiss actions
        // Uses NotificationConstants.GetStableId() for stable notification IDs
    }
}
#endif
```

**File:** `src/Awaitick/Services/ScheduledNotification/ScheduledNotificationService.Android.cs`

```csharp
#if __ANDROID__
public class ScheduledNotificationService : IScheduledNotificationService
{
    public void ScheduleCountdownNotification(EventCountdown eventCountdown)
    {
        // Uses AlarmManager.SetExactAndAllowWhileIdle for precise scheduling
        // Uses NotificationConstants.GetStableId() for PendingIntent request codes
    }

    public async Task<bool> RequestPermissionAsync()
    {
        // Android 13+: uses ActivityCompat.RequestPermissions for POST_NOTIFICATIONS
        // Older versions: permission always granted
    }
}
#endif
```

**MainActivity Deep Link Handling:**

```csharp
public class MainActivity : Microsoft.UI.Xaml.ApplicationActivity
{
    public static string? PendingCountdownId { get; set; }

    private void HandleIntent(Intent? intent)
    {
        var countdownId = intent?.GetStringExtra(NotificationAlarmReceiver.ExtraCountdownId);
        if (!string.IsNullOrEmpty(countdownId))
        {
            try
            {
                IoC.GetService<IDeepLinkService>()?.SetPendingNavigation(countdownId);
                IoC.GetService<IMessenger>()?.Send(new DeepLinkReceivedMessage());
            }
            catch
            {
                // Cold start: store for later consumption by App.xaml.cs
                PendingCountdownId = countdownId;
            }
        }
    }
}
```

---

### iOS

**Technology Stack:**
- `UNUserNotificationCenter` for scheduling
- `UNCalendarNotificationTrigger` with local time date components
- `UNNotificationAction` and `UNNotificationCategory` for actions
- `UNUserNotificationCenterDelegate` for handling responses

**File:** `src/Awaitick/Platforms/iOS/NotificationDelegate.cs`

```csharp
#if __IOS__
public class NotificationDelegate : UNUserNotificationCenterDelegate
{
    public static string? PendingCountdownId { get; set; }

    public override void DidReceiveNotificationResponse(...)
    {
        // Handles notification tap, snooze, and dismiss
        // Uses IoC for deep link navigation when available
        // Falls back to PendingCountdownId for cold start
    }
}
#endif
```

**File:** `src/Awaitick/Services/ScheduledNotification/ScheduledNotificationService.iOS.cs`

```csharp
#if __IOS__
public class ScheduledNotificationService : IScheduledNotificationService
{
    public void ScheduleCountdownNotification(EventCountdown eventCountdown)
    {
        // Checks suppressed notifications before scheduling
        // Converts DateTimeOffset to local time for NSDateComponents
        // Uses UNCalendarNotificationTrigger for precise scheduling
    }
}
#endif
```

**Register Delegate** (in `Main.iOS.cs`):

```csharp
UNUserNotificationCenter.Current.Delegate = new NotificationDelegate();
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
                        │  ViewNavigatedTo │   (initial load or
                        │  + DeepLink msg  │    DeepLinkReceivedMessage)
                        └────────┬─────────┘
                                 │
                        ┌────────▼─────────┐
                        │  Navigate to     │
                        │  CountdownDetail │
                        └──────────────────┘
```

**Cold Start Deep Links:**
- iOS: `NotificationDelegate.PendingCountdownId` → consumed in `App.xaml.cs` after IoC init
- Android: `MainActivity.PendingCountdownId` → consumed in `App.xaml.cs` after IoC init

**Warm Start Deep Links:**
- All platforms: `DeepLinkReceivedMessage` sent via `IMessenger` → `MainViewModel` handles navigation

**MainViewModel Integration:**

```csharp
public override async void ViewNavigatedTo(object? parameter)
{
    // ... load countdowns ...

    // Handle pending deep link (cold start)
    var pendingId = _deepLinkService.ConsumePendingNavigation();
    if (!string.IsNullOrEmpty(pendingId))
    {
        _navigationService.Navigate<CountdownDetailViewModel>(
            new CountdownDetailViewModel.NavigationModel(pendingId));
    }
}

// Handle deep link while already on MainView (warm start)
private static void DeepLinkReceivedHandler(object recipient, DeepLinkReceivedMessage message)
{
    var viewModel = recipient as MainViewModel;
    var pendingId = viewModel?._deepLinkService.ConsumePendingNavigation();
    if (!string.IsNullOrEmpty(pendingId))
    {
        viewModel._navigationService.Navigate<CountdownDetailViewModel>(
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

    // Initialize IoC for platform code
    Ioc.Default.ConfigureServices(Host.Services);
    IoC.SetProvider(Host.Services);

    // Consume cold-start deep links (iOS/Android)
    // ... platform-specific PendingCountdownId consumption ...

    await Host.Services.GetRequiredService<IDataService>().InitializeAsync();

    // Reschedule all future notifications
    var notificationService = Host.Services.GetRequiredService<IScheduledNotificationService>();
    var countdowns = await dataService.GetCountdownsAsync();
    await notificationService.RescheduleAllNotificationsAsync(
        countdowns.Where(c => c.TargetDateTime > DateTimeOffset.Now));

    // ... rest of initialization ...
}
```

---

## Files Summary

### Files

| File | Purpose |
|------|---------|
| `src/Awaitick.Core/Services/DeepLink/IDeepLinkService.cs` | Deep link interface |
| `src/Awaitick.Core/Services/DeepLink/DeepLinkService.cs` | Deep link implementation |
| `src/Awaitick.Core/Services/ScheduledNotification/NotificationConstants.cs` | Shared constants and stable ID helper |
| `src/Awaitick.Core/Messages/DeepLinkReceivedMessage.cs` | Message for warm-start deep links |
| `src/Awaitick/Services/ScheduledNotification/ScheduledNotificationService.Windows.cs` | Windows impl |
| `src/Awaitick/Services/ScheduledNotification/ScheduledNotificationService.Android.cs` | Android impl |
| `src/Awaitick/Services/ScheduledNotification/ScheduledNotificationService.iOS.cs` | iOS impl |
| `src/Awaitick/Platforms/Android/NotificationAlarmReceiver.cs` | Android alarm handler |
| `src/Awaitick/Platforms/iOS/NotificationDelegate.cs` | iOS notification delegate |

### Modified Files

| File | Changes |
|------|---------|
| `src/Awaitick.Core/Services/ScheduledNotification/IScheduledNotificationService.cs` | Add new methods |
| `src/Awaitick.Core/Services/ScheduledNotification/ScheduledNotificationService.others.cs` | Update stub |
| `src/Awaitick/App.xaml.cs` | Toast handler, DI registration, startup sync, IoC.SetProvider, cold-start deep links |
| `src/Awaitick/Platforms/Android/AndroidManifest.xml` | Permissions (no receiver declarations) |
| `src/Awaitick/Platforms/Android/MainActivity.Android.cs` | Intent handling with PendingCountdownId fallback |
| `src/Awaitick/Platforms/iOS/Main.iOS.cs` | Register notification delegate |
| `src/Awaitick.Core/ViewModels/MainViewModel.cs` | Handle pending deep link + DeepLinkReceivedMessage |

---

## Edge Cases

| Scenario | Handling |
|----------|----------|
| Past date | Skip scheduling silently |
| Permission denied | Return false from `RequestPermissionAsync()` |
| Device reboot | Reschedule on startup (all platforms) |
| App already running | `DeepLinkReceivedMessage` via `IMessenger` triggers navigation |
| Cold start deep link | Platform-specific `PendingCountdownId` consumed after IoC init |
| Time zone change | `DateTimeOffset` converted to local time for scheduling, reschedule on startup |
| Multiple notifications | Each gets unique stable ID based on countdown GUID |
| Countdown deleted | `UnscheduleCountdownNotification()` cancels pending notification |
| Countdown updated | Unschedule old, schedule new |
| Suppressed notification | `ScheduleCountdownNotification` skips suppressed countdowns (iOS) |

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
- [ ] Multiple concurrent countdowns
- [ ] Past countdown - no notification scheduled
