// WASM Notification Service - Web Notification API wrapper
var AwaitickNotificationService = {
    // Store scheduled timeouts
    scheduledNotifications: {},

    // Request notification permission
    requestPermission: function () {
        if (!("Notification" in window)) {
            console.error("This browser does not support desktop notification");
            return Promise.resolve("denied");
        }

        if (Notification.permission === "granted") {
            return Promise.resolve("granted");
        }

        if (Notification.permission !== "denied") {
            return Notification.requestPermission();
        }

        return Promise.resolve(Notification.permission);
    },

    // Schedule a notification
    scheduleNotification: function (id, title, body, targetDateTime, imageUrl) {
        try {
            // Clear any existing notification with the same ID
            this.unscheduleNotification(id);

            const targetTime = new Date(targetDateTime);
            const now = new Date();
            const delay = targetTime.getTime() - now.getTime();

            if (delay <= 0) {
                // Show notification immediately if the time has passed
                this.showNotification(title, body, imageUrl);
                return;
            }

            // Schedule the notification
            const timeoutId = setTimeout(() => {
                this.showNotification(title, body, imageUrl);
                delete this.scheduledNotifications[id];
            }, delay);

            this.scheduledNotifications[id] = timeoutId;
        } catch (error) {
            console.error("Error scheduling notification:", error);
        }
    },

    // Show a notification immediately
    showNotification: function (title, body, imageUrl) {
        if (Notification.permission !== "granted") {
            console.warn("Notification permission not granted");
            return;
        }

        try {
            const options = {
                body: body,
                icon: imageUrl || '/icon-512.png',
                badge: '/icon-192.png',
                tag: 'awaitick-countdown',
                requireInteraction: true,
                vibrate: [200, 100, 200]
            };

            const notification = new Notification(title, options);

            // Auto-close after 30 seconds
            setTimeout(() => {
                notification.close();
            }, 30000);

            // Handle notification click
            notification.onclick = function () {
                window.focus();
                notification.close();
            };
        } catch (error) {
            console.error("Error showing notification:", error);
        }
    },

    // Unschedule a notification
    unscheduleNotification: function (id) {
        try {
            if (this.scheduledNotifications[id]) {
                clearTimeout(this.scheduledNotifications[id]);
                delete this.scheduledNotifications[id];
            }
        } catch (error) {
            console.error("Error unscheduling notification:", error);
        }
    },

    // Clear all scheduled notifications
    clearAllNotifications: function () {
        try {
            for (const id in this.scheduledNotifications) {
                clearTimeout(this.scheduledNotifications[id]);
            }
            this.scheduledNotifications = {};
        } catch (error) {
            console.error("Error clearing notifications:", error);
        }
    },

    // Check if notifications are supported
    isSupported: function () {
        return "Notification" in window;
    },

    // Get current permission status
    getPermissionStatus: function () {
        if (!("Notification" in window)) {
            return "unsupported";
        }
        return Notification.permission;
    }
};
