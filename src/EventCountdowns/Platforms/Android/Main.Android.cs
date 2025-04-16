using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.UI.Xaml.Media;
using static Android.Telephony.CarrierConfigManager;

namespace EventCountdowns.Droid;
[global::Android.App.ApplicationAttribute(
    Label = "@string/ApplicationName",
    Icon = "@mipmap/icon",
    LargeHeap = true,
    HardwareAccelerated = true,
    Theme = "@style/AppTheme"
)]
public class Application : Microsoft.UI.Xaml.NativeApplication
{
	static Application()
	{
		CountdownsApp.InitializeLogging();
	}

	public Application(IntPtr javaReference, JniHandleOwnership transfer)
		: base(() => new CountdownsApp(), javaReference, transfer)
	{
	}
}

