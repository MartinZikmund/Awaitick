using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using EventCountdowns.Core.ViewModels;
using EventCountdowns.Views;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace EventCountdowns.Views
{
    public sealed partial class CountdownEditorView : CountdownEditorViewBase
    {
        public CountdownEditorView()
        {
            this.InitializeComponent();
        }
    }

    public partial class CountdownEditorViewBase : ViewBase<CountdownEditorViewModel>
    {
    }
}
